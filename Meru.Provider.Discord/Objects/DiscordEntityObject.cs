using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Meru.Common;

namespace Meru.Providers.Discord.Objects
{
    public class DiscordEntityObject : IEntityObject
    {
        private readonly ISnowflakeEntity entity;

        public object Id => entity.Id;
        public Type OriginalIdType => typeof(ulong);

        public DateTimeOffset CreatedAt => entity.CreatedAt;

        public DiscordEntityObject(ISnowflakeEntity entity)
        {
            this.entity = entity;
        }
    }
}
