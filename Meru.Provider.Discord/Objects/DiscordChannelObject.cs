using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Meru.Common;

namespace Meru.Providers.Discord.Objects
{
    public class DiscordChannelObject : DiscordEntityObject, IChannelObject
    {
        private readonly IChannel channel;
       
        public IGuildObject Guild => new DiscordGuildObject((channel as IGuildChannel).Guild);

        public DiscordChannelObject(IChannel channel) : base(channel)
        {
            this.channel = channel;
        }

        public async Task<IMessageObject> SendMessageAsync(string content)
        {
            return new DiscordMessageObject((await (channel as IMessageChannel).SendMessageAsync(content)));
        }
    }
}
