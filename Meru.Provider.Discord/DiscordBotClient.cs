using Meru.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Meru.Common.Providers;
using System.Threading.Tasks;
using Meru.Providers.Discord.Objects;
using Discord.WebSocket;

namespace Meru.Providers.Discord
{
    public class DiscordBotClient : IRunnable
    {
        public bool IsRunning { get; private set; } = false;
        
        public DiscordShardedClient client;

        public event Func<Task> OnBotStart;
        public event Func<Task> OnBotStop;

        public event Func<IBotProvider, Task> OnProviderConnect;
        public event Func<IBotProvider, Task> OnProviderDisconnect;

        public event Func<DiscordMessageObject, Task> OnMessageDelete;
        public event Func<DiscordMessageObject, DiscordChannelObject, Task> OnMessageEdit;
        public event Func<DiscordMessageObject, Task> OnMessageReceive;

        public event Func<DiscordUserObject, Task> OnUserJoin;
        public event Func<DiscordUserObject, Task> OnUserLeave;

        public event Func<DiscordUserObject, DiscordUserObject, Task> OnUserUpdate;



        public DiscordBotClient(DiscordProviderConfigurations config)
        {
            client.MessageReceived += async (msg) =>
            {
                await OnMessageReceive.Invoke(new DiscordMessageObject(msg));
            };

            client.MessageUpdated += async (msgCached, newMsg, channel) =>
            {
                await OnMessageEdit.Invoke(new DiscordMessageObject(newMsg), new DiscordChannelObject(channel));
            };
        }

        public async Task StartAsync()
        {
            await client.StartAsync();
        }

        public async Task StopAsync()
        {
            await client.StopAsync();
        }
    }
}
