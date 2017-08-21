using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Meru.Common;
using Discord.WebSocket;

namespace Meru.Providers.Discord.Objects
{
    public class DiscordMessageObject : DiscordEntityObject, IMessageObject
    {
        private readonly IMessage message;

        public IUserObject Author => new DiscordUserObject(message.Author);
        public IChannelObject Channel => new DiscordChannelObject(message.Channel as SocketChannel);
        public string Content => message.Content;

        public DiscordMessageObject(IMessage m) : base(m)
        {
            message = m;
        }

        public async Task ModifyAsync(string content)
        {
            await (message as IUserMessage).ModifyAsync((x) =>
            {
                x.Content = content;
            });
        }
    }
}
