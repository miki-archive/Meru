using System;
using System.Threading;
using System.Threading.Tasks;
using Meru.Common.Plugins;
using Meru.Common.Providers;
using Meru.Providers.Slack.Objects;
using SlackConnector;

namespace Meru.Providers.Slack
{
    public partial class SlackBotProvider : BaseExtendablePlugin, IBotProvider
    {
        internal static readonly ISlackConnector client = new SlackConnector.SlackConnector();
        internal static ISlackConnection connection;

        private string key = "";

        public SlackBotProvider(string key)
        {
            this.key = key; 
        }

        public async Task StartAsync()
        {
            ManualResetEventSlim clientReady = new ManualResetEventSlim(false);
            connection = await client.Connect(key);

            connection.OnMessageReceived += async message =>
            {
                await OnMessageReceive.Invoke(new SlackMessageObject(message));
            };
        }

        public async Task StopAsync()
        {
            await connection.Close();
        }
    }
}
