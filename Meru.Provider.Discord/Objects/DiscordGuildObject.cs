using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Meru.Common;

namespace Meru.Providers.Discord.Objects
{
    class DiscordGuildObject : DiscordEntityObject, IGuildObject
    {
        private readonly IGuild guild;

        public string Name => guild.Name;

        public Type OriginalIdType = typeof(ulong);

        public DiscordGuildObject(IGuild guild) : base(guild)
        {
            this.guild = guild;
        }
    }
}
