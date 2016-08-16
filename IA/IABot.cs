using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Diagnostics;
using IA.Events;
using System.Threading;
using IA.SQL;
using System.IO;
using IA.FileHandling;

namespace IA
{
    public sealed class IABot : IDisposable
    {
        public ClientInformation clientInformation { private set; get; } = new ClientInformation();

        public DiscordClient Client { private set; get; }
        public EventSystem Events { private set; get; }
        public SQLManager Sql { private set; get; }

        public const string VersionText = "IA v" + VersionNumber;
        public const string VersionNumber = "1.3.1";

        FileWriter crashLog;

        string CurrentPath = Directory.GetCurrentDirectory();

        public IABot()
        {
            ClientInformation clientInfo;

            if (!File.Exists(CurrentPath + "/preferences.config"))
            {
                clientInfo = InitializePreferencesFile();
            }
            else
            {
                clientInfo = LoadPreferenceFile();
            }
            Events = new EventSystem(x =>
            {
                x.Name = clientInformation.botName;
                x.Identifier = clientInformation.botIdentifier;
                x.SqlInformation = clientInformation.sqlInformation;
            });
            clientInformation = clientInfo;
            Sql = new SQLManager(clientInformation.sqlInformation, clientInformation.botIdentifier);
            crashLog = new FileWriter("crashLog_" + DateTime.Now.ToLongDateString());
            Start();
        }
        public IABot(Action<ClientInformation> client)
        {
            if (client != null)
            {
                client.Invoke(clientInformation);
                Events = new EventSystem(x =>
                {
                    x.Name = clientInformation.botName;
                    x.Identifier = clientInformation.botIdentifier;
                    x.SqlInformation = clientInformation.sqlInformation;
                });
                Sql = new SQLManager(clientInformation.sqlInformation, clientInformation.botIdentifier);
                Start();
            }
        }

        void Start()
        {
            if (clientInformation.CanLog(LogLevel.INFO)) Log.Message("Starting " + VersionText);
            Client = new DiscordClient(x =>
            {
                x.AppName = clientInformation.botName;
                x.AppVersion = clientInformation.botVersion;
            });
            Client.MessageReceived += Client_MessageReceived;
            Client.UserJoined += Client_UserJoined;
            Client.UserLeft += Client_UserLeft;
            Client.Ready += Client_Ready;
        }



        public void AddDeveloper(ulong developerId)
        {
            Events.developers.Add(developerId);
        }
        public void AddDeveloper(User user)
        {
            Events.developers.Add(user.Id);
        }

        [Obsolete("use IABot.EventSystem.AddCommandEvent()")]
        public void AddEvent(Action<EventInformation> e)
        {
            Events.AddCommandEvent(e);
        }

        public void Connect()
        {
            if(clientInformation.botToken == "")
            {
                Log.ErrorAt("Connect", "No Discord token found in bot properties.");
                return;
            }

            try
            {
                Client.ExecuteAndWait(async () =>
                {
                    await Client.Connect(clientInformation.botToken);
                });
            }
            catch(Exception e)
            {
                Log.ErrorAt("Connect", "Token was refused.");
                crashLog.Write(e.Message + "\n" + e.StackTrace);
            }
        }

        public void Dispose()
        {
            crashLog.Dispose();
            GC.SuppressFinalize(this);
        }

        public Event GetEvent(string id)
        {
            return Events.GetEvent(id);
        }
        public string GetVersion()
        {
            return VersionText;
        }


        private void Client_Ready(object sender, EventArgs e)
        {
            Log.DoneAt(clientInformation.botName, "Connected to discord!");
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.RawText.Contains(Client.CurrentUser.Mention) || e.Message.RawText.Contains(Client.CurrentUser.NicknameMention))
            {
                await Events.OnMention(e);
                return;
            }
            await Events.OnMessageRecieved(e);
        }

        private async void Client_UserLeft(object sender, UserEventArgs e)
        {
            await Events.OnUserLeave(e);
        }

        private async void Client_UserJoined(object sender, UserEventArgs e)
        {
            await Events.OnUserJoin(e);
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
    }
}
