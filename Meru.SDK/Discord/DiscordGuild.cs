using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class DiscordGuild : IDiscordGuild
    {
        public virtual string AvatarUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual int ChannelCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ulong Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual IDiscordUser Owner
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual int UserCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual List<IDiscordRole> Roles
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int VoiceChannelCount => throw new NotImplementedException();

        public virtual Task<IDiscordUser> GetUserAsync(ulong user_id)
        {
            throw new NotImplementedException();
        }

        public virtual IDiscordRole GetRole(ulong role_id)
        {
            throw new NotImplementedException();
        }

        Task<List<IDiscordMessageChannel>> IDiscordGuild.GetChannels()
        {
            throw new NotImplementedException();
        }

        Task<IDiscordMessageChannel> IDiscordGuild.GetDefaultChannel()
        {
            throw new NotImplementedException();
        }
    }
}