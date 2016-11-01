using Discord;
using IA.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IA
{
    public static class SDKHelper
    {
        public static DiscordGuild ToSDKGuild(this IGuild guild)
        {
            DiscordGuild g = new DiscordGuild();


            return g;
        }
    }
}
