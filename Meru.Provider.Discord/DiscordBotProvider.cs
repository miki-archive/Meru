using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Meru.Common;
using Meru.Common.Plugins;
using Meru.Common.Providers;
using Meru.Providers.Discord.Objects;

namespace Meru.Providers.Discord
{
    public partial class DiscordBotProvider : BaseExtendablePlugin, IBotProvider
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
                await OnMessageReceive.Invoke(new DiscordMessageObject(message));
            };
        }

        public override async Task StartAsync()
        {
            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();
            await base.StartAsync();
        }

        public override async Task StopAsync()
        {
            await client.StopAsync();
            await base.StopAsync();
        }
    }
}
