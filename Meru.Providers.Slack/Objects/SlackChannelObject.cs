using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;
using SlackConnector.Models;

namespace Meru.Providers.Slack.Objects
{
    class SlackChannelObject : IChannelObject
    {
        private SlackChatHub channel;

        public SlackChannelObject(SlackChatHub channel)
        {
            this.channel = channel;
        }

        public IGuildObject Guild => throw new NotImplementedException();

        public object Id => channel.Id;

        public Type OriginalIdType => typeof(int);

        public DateTimeOffset CreatedAt => DateTime.Now;

        public async Task<IMessageObject> SendMessageAsync(string message)
        {
            IMessageObject m = null;
            channel
            return m;
        }

        private async Task<SlackChatHub> GetChatHub(ResponseMessage responseMessage)
        {
            SlackChatHub chatHub = null;

            if (responseMessage.ResponseType == ResponseType.Channel)
            {
                chatHub = new SlackChatHub { Id = responseMessage.Channel };
            }
            else if (responseMessage.ResponseType == ResponseType.DirectMessage)
            {
                if (string.IsNullOrEmpty(responseMessage.Channel))
                {
                    chatHub = await GetUserChatHub(responseMessage.UserId);
                }
                else
                {
                    chatHub = new SlackChatHub { Id = responseMessage.Channel };
                }
            }

            return chatHub;
        }
    }
}
