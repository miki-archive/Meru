using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Diagnostics;
using IA.Events;
using System.Threading;
using IA.Logging;
using IA.SQL;

namespace IA
{
    public class IABot
    {
        public ClientInformation clientInformation { private set; get; } = new ClientInformation();

        public DiscordClient client { private set; get; }
        public SQLManager sql { private set; get; }
        EventSystem events;

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
                events = new EventSystem(x =>
                {
                    x.Name = clientInformation.botName;
                    x.Identifier = clientInformation.botIdentifier;
                    x.SqlInformation = clientInformation.sqlInformation;
                });
                sql = new SQLManager(clientInformation.sqlInformation, clientInformation.botIdentifier);
                Start();
            }
        }

        void Start()
        {
            if (clientInformation.CanLog(LogLevel.INFO)) Log.Message("Starting " + VersionText);
            client = new DiscordClient(x =>
            {
                x.AppName = clientInformation.botName;
                x.AppVersion = clientInformation.botVersion;
            });
            client.MessageReceived += Client_MessageReceived;
            client.UserJoined += Client_UserJoined;
            client.Ready += Client_Ready;
        }

        private async void Client_UserJoined(object sender, UserEventArgs e)
        {
            await events.OnUserJoin(e);
        }

        public void AddDeveloper(ulong developerId)
        {
            events.developers.Add(developerId);
        }
        public void AddDeveloper(User user)
        {
            events.developers.Add(user.Id);
        }

        public void AddEvent(Action<EventInformation> e)
        {
            events.AddEvent(e);
        }
        public void AddCommandEvent(Action<EventInformation> e)
        {
            events.AddEvent(e);
        }

        public void Connect()
        {
            if(clientInformation.botToken == "")
            {
                Log.Error("No Discord token found in bot properties.");
                return;
            }

            client.ExecuteAndWait(async () =>
            {
                await client.Connect(clientInformation.botToken);
            });
        }

        public void DisableEvent(MessageEventArgs e)
        {
            events.Toggle(e, false);
        }

        public void EnableEvent(MessageEventArgs e)
        {
            events.Toggle(e, true);
        }

        /// <summary>
        /// Returns total usage of all events.
        /// </summary>
        public int GetEventUses()
        {
            return events.CommandsUsed();
        }

        /// <summary>
        /// Returns usage from specific event.
        /// </summary>
        /// <param name="eventName">name of event</param>
        public int GetEventUses(string eventName)
        {
            return events.CommandsUsed(eventName);
        }

        public void IgnoreUser(ulong userId)
        {
            events.Ignore(userId);
        }

        public string ListEvents(MessageEventArgs e)
        {
            return events.ListCommands(e).Result;
        }
        public async Task<string> ListEventsAsync(MessageEventArgs e)
        {
            return await events.ListCommands(e);
        }

        public void ToggleEvent(MessageEventArgs e)
        {
            events.Toggle(e);
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.DoneAt(clientInformation.botName, "Connected to discord!");
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            await events.OnMessageRecieved(e);
        }
    }
}
