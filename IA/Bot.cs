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

        string CurrentPath = Directory.GetCurrentDirectory();
        

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
            InitializeBot();
        }

        public Bot(Action<ClientInformation> info)
        {
            clientInformation = new ClientInformation();
            info.Invoke(clientInformation);
            InitializeBot();
        }

        public void Connect()
        {
            Client.LoginAsync(TokenType.Bot, clientInformation.botToken);
            Client.ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            await Client.LoginAsync(TokenType.Bot, clientInformation.botToken);
            await Client.ConnectAsync();
        }

        public void AddDeveloper(ulong developerId)
        {
            Events.developers.Add(developerId);
        }
        public void AddDeveloper(IUser user)
        {
            Events.developers.Add(user.Id);
        }

        [Obsolete("use IABot.Events.AddCommandEvent(...)")]
        public void AddEvent(Action<Event> e)
        {
            Events.AddCommandEvent(e);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Event GetEvent(string id)
        {
            return Events.GetEvent(id);
        }

        void InitializeBot()
        { 
            Client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                ShardId = 0,
                LogLevel = LogSeverity.Info,
                TotalShards = clientInformation.shardCount
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

        private async Task Client_MessageReceived(IMessage arg)
        {
            IGuild guild = (arg.Channel as IGuildChannel)?.Guild;
            if (guild != null)
            {
                if (arg.MentionedUsers.Contains(await Client.GetCurrentUserAsync()))
                {
                    await Events.OnMention(arg, guild);
                }
                await Events.OnMessageRecieved(arg, guild);
            }
            else
            {
                await Events.OnPrivateMessage(arg);
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
