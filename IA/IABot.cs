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

namespace IA
{
    public class IABot
    {
        public ClientInformation clientInformation { private set; get; } = new ClientInformation();

        public DiscordClient Client { private set; get; }
        public SQLManager Sql { private set; get; }
        public EventSystem Events { private set; get; }

        public const string VersionText = "IA v" + VersionNumber;
        public const string VersionNumber = "1.3";

        public IABot()
        {
            
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

        private async void Client_UserLeft(object sender, UserEventArgs e)
        {
            await Events.OnUserLeave(e);
        }

        private async void Client_UserJoined(object sender, UserEventArgs e)
        {
            await Events.OnUserJoin(e);
        }

        public void AddDeveloper(ulong developerId)
        {
            Events.developers.Add(developerId);
        }
        public void AddDeveloper(User user)
        {
            Events.developers.Add(user.Id);
        }

        [Obsolete("use IABot.EventSystem.AddCommandEvent()", true)]
        public void AddEvent(Action<EventInformation> e)
        {
        }

        public void Connect()
        {
            if(clientInformation.botToken == "")
            {
                Log.Error("No Discord token found in bot properties.");
                return;
            }

            Client.ExecuteAndWait(async () =>
            {
                await Client.Connect(clientInformation.botToken);
            });
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.DoneAt(clientInformation.botName, "Connected to discord!");
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.RawText.StartsWith(Client.CurrentUser.Mention))
            {
                await Events.OnMention(e);
                return;
            }
            await Events.OnMessageRecieved(e);
        }
    }
}
