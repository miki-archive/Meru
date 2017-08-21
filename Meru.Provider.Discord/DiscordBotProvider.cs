using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Meru.Common;
using Meru.Common.Providers;
using Meru.Providers.Discord.Objects;

namespace Meru.Providers.Discord
{
    public partial class DiscordBotProvider : IBotProvider
    {
        private DiscordShardedClient client;
        private DiscordProviderConfigurations config;

        public DiscordBotProvider(DiscordProviderConfigurations config)
        {
            this.config = config;

            client = new DiscordShardedClient(
                new DiscordSocketConfig
                {
                    TotalShards = config.ShardCount
                });

            client.MessageReceived += async message =>
            {
                await OnMessageReceived.Invoke(new DiscordMessageObject(message));
            };
        }

        public async Task StartAsync()
        {
            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();
        }

        public async Task StopAsync()
        {
            await client.StopAsync();
        }
    }
}
