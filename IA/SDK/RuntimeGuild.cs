using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeGuild : DiscordGuild
    {
        public IGuild guild = null;

        public override string AvatarUrl
        {
            get
            {
                return guild.IconUrl;
            }
        }

        public override ulong Id
        {
            get
            {
                return guild.Id;
            }
        }

        public override string Name
        {
            get
            {
                return guild.Name;
            }
        }

        public override uint ChannelCount
        {
            get
            {
                return (uint)guild.GetChannelsAsync().GetAwaiter().GetResult().Count;
            }
        }

        public override uint UserCount
        {
            get
            {
                return (uint)guild.GetUsersAsync().GetAwaiter().GetResult().Count;
            }
        }

        public override IDiscordUser Owner
        {
            get
            {
                return new RuntimeUser(guild.GetOwnerAsync().GetAwaiter().GetResult());
            }
        }

        public override List<IDiscordRole> Roles
        {
            get
            {
                List<IDiscordRole> output = new List<IDiscordRole>();

                foreach (IRole role in guild.Roles)
                {
                    output.Add(new RuntimeRole(role));
                }

                return output;
            }
        }

        public RuntimeGuild(IGuild g)
        {
            guild = g;
        }

        public override async Task<IDiscordChannel> GetDefaultChannel()
        {
            return new RuntimeChannel(await guild.GetDefaultChannelAsync());
        }

        public override async Task<IDiscordUser> GetUserAsync(ulong user_id)
        {
            return new RuntimeUser(await guild.GetUserAsync(user_id));
        }

        public override async Task<List<IDiscordChannel>> GetChannels()
        {
            List<IGuildChannel> channels = (await guild.GetChannelsAsync()).ToList();
            List<IDiscordChannel> rChannels = new List<IDiscordChannel>();
            foreach(IGuildChannel c in channels)
            {
                rChannels.Add(new RuntimeChannel(c));
            }
            return rChannels;
        }

        public override IDiscordRole GetRole(ulong role_id)
        {
            return new RuntimeRole(guild.GetRole(role_id));
        }
    }
}
