using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.Events;
using IA.FileHandling;
using IA.SQL;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using IA.Internal;
using IA.Addons;
using IA.SDK;
using IA.SDK.Interfaces;
using System.Windows.Forms;
using System.Threading;
using System.Net.Http;

namespace IA
{
    public class Bot
    {
        public AddonManager Addons { private set; get; }

        public ShardedClient Client { private set; get; }

        public EventSystem Events { private set; get; }
        public MySQL Sql { private set; get; }

        public string Name
        {
            get
            {
                return clientInformation.Name;
            }
        }

        public string Version
        {
            get
            {
                return clientInformation.Version;
            }
        }

        public const string VersionText = "IA v" + VersionNumber;
        public const string VersionNumber = "1.5.5";

        public static Bot instance;

        private ClientInformation clientInformation;

        private string currentPath = Directory.GetCurrentDirectory();

        public Bot()
        {
            if (!File.Exists(currentPath + "/preferences.config"))
            {
                clientInformation = InitializePreferencesFile();
            }
            else
            {
                clientInformation = LoadPreferenceFile();
            }
            InitializeBot().GetAwaiter().GetResult();
        }
        public Bot(Action<ClientInformation> info)
        {
            clientInformation = new ClientInformation();
            info.Invoke(clientInformation);
            InitializeBot().GetAwaiter().GetResult();
        }

        public void AddDeveloper(ulong id)
        {
            Events.Developers.Add(id);
        }
        public void AddDeveloper(IDiscordUser user)
        {
            Events.Developers.Add(user.Id);
        }
        public void AddDeveloper(IUser user)
        {
            Events.Developers.Add(user.Id);
        }

        public async Task ConnectAsync()
        {
            await Client.ConnectAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public int GetShardId()
        {
            return clientInformation.ShardId;
        }

        public int GetTotalShards()
        {
            return clientInformation.ShardCount;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.ErrorAt(ex.Source, ex.Message);
            if (e.IsTerminating)
            {
                Log.Error("Closing Shard");
            }
        }

        private ClientInformation InitializePreferencesFile()
        {
            ClientInformation outputBotInfo = new ClientInformation();
            FileWriter file = new FileWriter("preferences", "config");
            file.WriteComment(VersionText + " preferences file");
            file.WriteComment("Please do not change this file except to change\n# except to change your settings");
            file.WriteComment("Bot Name");
            Console.WriteLine("Enter bot name: ");
            string inputString = Console.ReadLine();
            file.Write(inputString);
            outputBotInfo.Name = inputString;

            file.WriteComment("Bot Token");
            Console.WriteLine("Enter bot token: ");
            inputString = Console.ReadLine();
            file.Write(inputString);
            outputBotInfo.Token = inputString;

            file.WriteComment("Shard count");
            Console.WriteLine("Shards [1-25565]:");
            inputString = Console.ReadLine();
            outputBotInfo.ShardCount = int.Parse(inputString);
            if (outputBotInfo.ShardCount < 1)
            {
                outputBotInfo.ShardCount = 1;
            }
            else if (outputBotInfo.ShardCount > 25565)
            {
                outputBotInfo.ShardCount = 25565;
            }

            file.Finish();

            return outputBotInfo;
        }

        private ClientInformation LoadPreferenceFile()
        {
            ClientInformation outputBotInfo = new ClientInformation();
            FileReader file = new FileReader("preferences", "config");
            outputBotInfo.Name = file.ReadLine();
            outputBotInfo.Token = file.ReadLine();
            file.Finish();
            return outputBotInfo;
        }

        private async Task InitializeBot()
        {
            instance = this;

            Log.InitializeLogging(clientInformation);

            Client = new ShardedClient(clientInformation);

            Events = new EventSystem(x =>
            {
                x.Name = clientInformation.Name;
                x.Identifier.Value = clientInformation.Prefix.Value;
                x.SqlInformation = clientInformation.sqlInformation;
            });

            Sql = new MySQL(clientInformation.sqlInformation, clientInformation.Prefix);

            Addons = new AddonManager();

            if (clientInformation.EventLoaderMethod != null)
            {
                await clientInformation.EventLoaderMethod(this);
            }

            Client.MessageRecieved += Client_MessageReceived;
            Client.Ready += Client_Ready;
            //x.JoinedGuild += Client_JoinedGuild;
            //x.LeftGuild += Client_LeftGuild;
            //x.Log += Client_Log;
        }

        // Events
        private async Task Client_JoinedGuild(IGuild arg)
        {
            RuntimeGuild g = new RuntimeGuild(arg);

            Task.Run(() => Events.OnGuildJoin(g));
        }

        private async Task Client_LeftGuild(IGuild arg)
        {
            RuntimeGuild g = new RuntimeGuild(arg);

            Task.Run(() => Events.OnGuildLeave(g));
        }

        private async Task Client_Disconnected(Exception arg)
        {
            Log.Error("Disconnected!");
            await Task.CompletedTask;
        }

        private async Task Client_Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            await Task.CompletedTask;
        }

        private async Task Client_Ready(int id)
        {
            Log.Done($"Shard {id} Connected!");
            await Task.CompletedTask;
        }

        private async Task Client_MessageReceived(IDiscordMessage r)
        {
            if (r.Content.Contains(r.Bot.Id.ToString()))
            {
                await Task.Run(() => { Events.OnMention(r); });
            }

            if (r.Guild != null)
            {
                await Task.Run(() => { Events.OnMessageRecieved(r); });
            }
            else
            {
                await Task.Run(() => { Events.OnPrivateMessage(r); });
            }
        }
    }
}
