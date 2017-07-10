using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Events
{
    public class EventContext
    {
        public string arguments;
        public ICommandHandler commandHandler;
        public IDiscordMessage message;

        public IDiscordUser Author => message.Author;
        public IDiscordMessageChannel Channel => message.Channel;
        public IDiscordGuild Guild => message.Guild;
    }
}
