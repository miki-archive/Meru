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
using IA.Forms;
using System.Threading;

namespace IA
{
    public class Bot
    {
        public ClientInformation clientInformation { private set; get; }

        public AddonManager Addons { private set; get; }
        public DiscordSocketClient Client { private set; get; }
        public EventSystem Events { private set; get; }
        public MySQL Sql { private set; get; }

        public int ShardId
        {
            get
            {
                return clientInformation.ShardId;
            }
        }

        public const string VersionText = "IA v" + VersionNumber;
        public const string VersionNumber = "1.4.4";

        public bool isManager = false;

        public static Bot instance;

        string currentPath = Directory.GetCurrentDirectory();

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

        public void AddDeveloper(ulong developerId)
        {
            Events.Developers.Add(developerId);
        }
        public void AddDeveloper(IUser user)
        {
            Events.Developers.Add(user.Id);
        }

        public void Connect()
        {
            if (!isManager)
            {
                Log.Message("Connecting...");
                Client.LoginAsync(TokenType.Bot, clientInformation.Token).GetAwaiter().GetResult();
                Client.ConnectAsync().GetAwaiter().GetResult();
                Task.Delay(-1).GetAwaiter().GetResult();
            }
        }

        public async Task ConnectAsync()
        {
            if (!isManager)
            {
                Log.Message("Connecting...");
                await Client.LoginAsync(TokenType.Bot, clientInformation.Token);
                await Client.ConnectAsync();
                await Task.Delay(-1);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Obsolete("Use Events.(their respective event)(); for more performance.")]
        public Event GetEvent(string id)
        {
            return Events.GetEvent(id);
        }

        public int GetShardId()
        {
            return clientInformation.ShardId;
        }

        private async Task InitializeBot()
        {
            int id = 0;
            instance = this;

            Log.InitializeLogging(clientInformation);

            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                ShardId = id,
                LogLevel = LogSeverity.Critical,
                TotalShards = clientInformation.ShardCount
            });

            Events = new EventSystem(x =>
            {
                x.Name = clientInformation.Name;
                x.Identifier.Value = clientInformation.Prefix.Value;
                x.SqlInformation = clientInformation.sqlInformation;
            });

            Sql = new MySQL(clientInformation.sqlInformation, clientInformation.Prefix);

            Addons = new AddonManager();

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Log.Message(string.Join(" | ", Environment.GetCommandLineArgs()));
                id = int.Parse(Environment.GetCommandLineArgs()[1]);
                Console.Title = "Shard " + id;
            }
            else
            {
                Console.Title = clientInformation.Name + " " + clientInformation.Version;
                if (Debugger.IsAttached)
                {
                    isManager = false;
                    clientInformation.ShardCount = 1;
                }
                else
                {
                    isManager = true;
                    new Manager(clientInformation.ShardCount);
                }
            }

            if (!isManager)
            {
                clientInformation.ShardId = id;
                await Addons.Load(this);

                Client.MessageReceived += Client_MessageReceived;
                Client.JoinedGuild += Client_JoinedGuild;
                Client.LeftGuild += Client_LeftGuild;
                Client.Ready += Client_Ready;
                Client.Disconnected += Client_Disconnected;
                Client.Log += Client_Log;
            }
        }

        private async Task Client_JoinedGuild(IGuild arg)
        {
            await Events.OnGuildJoin(arg);
        }

        private async Task Client_LeftGuild(IGuild arg)
        {
            await Events.OnGuildLeave(arg);
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

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.ErrorAt(ex.Source, ex.Message);
            if(e.IsTerminating)
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
            else if(outputBotInfo.ShardCount > 25565)
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

        private async Task Client_Ready()
        {
            Log.Done("Connected!");
            await Task.CompletedTask;
        }

        private async Task Client_MessageReceived(IMessage arg)
        {
            RuntimeMessage r = new RuntimeMessage(arg);

            if (r.Guild != null)    
            {
                await Task.Run(async () => await Events.OnMessageRecieved(r));
            }
            else
            {
                await Task.Run(async () => await Events.OnPrivateMessage(r));
            }

                            if (arg.Content.Contains(Client.CurrentUser.Id.ToString()))
                {
                    await Task.Run(async () => await Events.OnMention(r));
                }
        }
    }
}
