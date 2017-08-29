using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Meru.Common;

namespace Meru.Providers.Discord.Objects
{
    public class DiscordUserObject : DiscordEntityObject, IUserObject
    {
        private readonly IUser user;

        public DiscordUserObject(IUser user) : base(user)
        {
            this.user = user;
        }
    }
}
