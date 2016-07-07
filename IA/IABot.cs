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

namespace IA
{
    public class IABot
    {
        public ClientInformation clientInformation = new ClientInformation();

        public DiscordClient client;
        EventSystem events;

        static string version = "1.2";

        public IABot(Action<ClientInformation> client)
        {
            if (client != null)
            {
                client.Invoke(clientInformation);
                events = new EventSystem(x =>
                {
                    x.Name = clientInformation.botName;
                    x.SqlInformation = clientInformation.sqlInformation;
                });
                Start();
            }
        }

        void Start()
        {
            if (clientInformation.CanLog(LogLevel.INFO)) Log.Message("Starting IA v" + version);
            client = new DiscordClient(x =>
            {
                x.AppName = clientInformation.botName;
                x.AppVersion = clientInformation.botVersion;
            });
            client.MessageReceived += Client_MessageReceived;
        }
 
        public void AddEvent(Action<EventInformation> e)
        {
            events.AddEvent(e);
        }

        public void Connect()
        {
            client.ExecuteAndWait(async () =>
            {
                await client.Connect(Global.ApiKey);
            });
        }

        public void DisableEvent(MessageEventArgs e)
        {
            events.Disable(e);
        }
        public void EnableEvent(MessageEventArgs e)
        {
            events.Enable(e);
        }

        public int GetEventUses()
        {
            return events.CommandsUsed();
        }

        //TODO
        //public int GetEventUses(string eventName)
        //{
            
        //}

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

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            await events.Check(e);
        }
    }
}
