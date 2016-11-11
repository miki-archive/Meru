using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeGuild : DiscordGuild
    {
        public IGuild guild;

        public RuntimeGuild(IGuild g)
        {
            guild = g;
        }

        public override async Task<DiscordChannel> GetDefaultChannel()
        {
            return new RuntimeChannel(await guild.GetDefaultChannelAsync());
        }
    }
}
