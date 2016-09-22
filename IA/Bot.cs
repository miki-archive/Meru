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
using static IA.BotWin32Window;

namespace IA
{
    public class Bot
    {
        public ClientInformation clientInformation { private set; get; }

        public DiscordSocketClient Client { private set; get; }
        public EventSystem Events { private set; get; }
        public SQLManager Sql { private set; get; }

        public const string VersionText = "IA v" + VersionNumber;
        public const string VersionNumber = "1.4";

        public int shardId = -1;

        string CurrentPath = Directory.GetCurrentDirectory();
        List<Shard> shard = new List<Shard>();
        bool isManager = false;

        AppDomain app = AppDomain.CurrentDomain;

        public Bot()
        {
            if (!File.Exists(CurrentPath + "/preferences.config"))
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
            Log.Message("Running " + VersionText);
            clientInformation = new ClientInformation();
            info.Invoke(clientInformation);
            InitializeBot().GetAwaiter().GetResult();
        }

        public void Connect()
        {
            if (!isManager)
            {
                Client.LoginAsync(TokenType.Bot, clientInformation.botToken);
                Client.ConnectAsync();
            }
        }

        public async Task ConnectAsync()
        {
            if (!isManager)
            {
                Log.Message("Connecting...");
                await Client.LoginAsync(TokenType.Bot, clientInformation.botToken);
                await Client.ConnectAsync();
            }
        }

        public void AddDeveloper(ulong developerId)
        {
            Events.developers.Add(developerId);
        }
        public void AddDeveloper(IUser user)
        {
            Events.developers.Add(user.Id);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Event GetEvent(string id)
        {
            return Events.GetEvent(id);
        }

        public int GetShardId()
        {
            return shardId;
        }

        private async Task Heartbeat()
        {
            for (int i = 0; i < shard.Count; i++)
            {
                if(!shard[i].shardProcess.Responding)
                {
                    Log.Error("[Shard " + i + "] has stopped responding.");
                    shard[i].shardProcess.Kill();
                    shard[i] = new Shard(i);
                }

                if (shard[i].shardProcess.HasExited)
                {
                    Log.Error("[Shard " + i + "] has crashed.");
                    shard[i] = new Shard(i);
                }
            }

            await Task.Delay(1000);
            await Heartbeat();
        }

        async Task InitializeBot()
        {
            int id = 0;

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                Log.Message(string.Join(" | ", Environment.GetCommandLineArgs()));
                id = int.Parse(Environment.GetCommandLineArgs()[1]);
                Console.Title = "Shard " + id;
            }
            else
            {
                if (Debugger.IsAttached)
                {
                    id = 1;
                    clientInformation.shardCount = 1;
                    Log.Warning("Set shardcount to 1 for debugging purposes.");
                }
                else
                {
                    Console.Title = "Miki " + clientInformation.botVersion;
                    isManager = true;
                }
            }

            if (!isManager)
            {
                shardId = id;
                Client = new DiscordSocketClient(new DiscordSocketConfig()
                {
                    ShardId = id,
                    LogLevel = LogSeverity.Info,
                    TotalShards = clientInformation.shardCount,   
                });

                Events = new EventSystem(x =>
                {
                    x.Name = clientInformation.botName;
                    x.Identifier = clientInformation.botIdentifier;
                    x.SqlInformation = clientInformation.sqlInformation;
                });
                Sql = new SQLManager(clientInformation.sqlInformation, clientInformation.botIdentifier);

                Client.MessageReceived += Client_MessageReceived;
                Client.UserLeft += Client_UserLeft;
                Client.UserJoined += Client_UserJoined;
                Client.Ready += Client_Ready;
            }
            else
            {
                app.UnhandledException += App_UnhandledException;
                app.ProcessExit += App_ProcessExit;
                BotWin32Window.SetConsoleCtrlHandler(new BotWin32Window.HandlerRoutine(App_OnConsoleWindowClose), true);

                Process[] p = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                foreach(Process px in p)
                {
                    if(px.Id != Process.GetCurrentProcess().Id)
                    {
                        px.Kill();
                    }
                }

                for (int i = 0; i < clientInformation.shardCount; i++)
                {
                    shard.Add(new Shard(i));
                }
                await Task.Run(async () => await Heartbeat());
                await Task.Delay(-1);
            }
        }

        private async Task Client_Disconnected(Exception arg)
        {
            Log.Message("Disconnected!");
            await Task.Delay(-1);   
        }

        private async Task Client_Log(LogMessage arg)
        {
            Log.Message(arg.Message);
            await Task.Delay(-1);
        }   

        private bool App_OnConsoleWindowClose(CtrlTypes ctrlType)
        {
            foreach (Shard b in shard)
            {
                b.shardProcess.Kill();
            }

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    Console.WriteLine("User is logging off!");
                    break;

            }
            return true;
        }

        private void App_ProcessExit(object sender, EventArgs e)
        {
            foreach(Shard b in shard)
            {
                b.shardProcess.Kill();
            }
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
            outputBotInfo.botName = inputString;

            file.WriteComment("Bot Token");
            Console.WriteLine("Enter bot token: ");
            inputString = Console.ReadLine();
            file.Write(inputString);
            outputBotInfo.botToken = inputString;

            file.Finish();

            return outputBotInfo;
        }

        private ClientInformation LoadPreferenceFile()
        {
            ClientInformation outputBotInfo = new ClientInformation();
            FileReader file = new FileReader("preferences", "config");
            outputBotInfo.botName = file.ReadLine();
            outputBotInfo.botToken = file.ReadLine();
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
            if (shardId == 0) Log.Message("Message recieved!");
            IGuild guild = (arg.Channel as IGuildChannel)?.Guild;
            if (guild != null)
            {
                if (arg.Content.Contains((await Client.GetCurrentUserAsync()).Id.ToString()))
                {
                    await Events.OnMention(arg, guild);
                }
                await Events.OnMessageRecieved(arg, guild);
            }
        }

        private async Task Client_UserJoined(IGuildUser user)
        {
            await Events.OnUserJoin(user);
        }

        private async Task Client_UserLeft(IGuildUser user)
        {
            await Events.OnUserLeave(user);
        }
    }
}
