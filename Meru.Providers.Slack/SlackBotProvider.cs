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
        internal static readonly ISlackConnector Client = new SlackConnector.SlackConnector();
        internal static ISlackConnection Connection;

        private readonly string _key;

        public SlackBotProvider(string key)
        {
            _key = key; 
        }

        public override async Task StartAsync()
        {
            Connection = await Client.Connect(_key);

            Connection.OnMessageReceived += async message =>
            {
                await OnMessageReceive.Invoke(new SlackMessageObject(message));
            };
            await base.StartAsync();
        }

        public override async Task StopAsync()
        {
            await Connection.Close();
            await base.StopAsync();
        }
    }
}
