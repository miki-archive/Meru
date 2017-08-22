using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;
using SlackConnector.Models;

namespace Meru.Providers.Slack.Objects
{
    public class SlackMessageObject : IMessageObject
    {
        private SlackMessage message;

        public IUserObject Author => throw new NotImplementedException();

        public IChannelObject Channel => new SlackChannelObject(message.ChatHub);// new SlackChannelObject(SlackBotProvider.client.Channels.FirstOrDefault(x => x.id == message.channel));

        public string Content => message.Text;

        public object Id => message;

        public Type OriginalIdType => typeof(int);

        public DateTimeOffset CreatedAt => DateTime.Now;

        public SlackMessageObject(SlackMessage message)
        {
            this.message = message;
        }

        public async Task ModifyAsync(string message)
        {
            await Channel.SendMessageAsync(message);
        }
    }
}
