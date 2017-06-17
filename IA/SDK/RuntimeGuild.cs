using Discord;
using IA.SDK.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace IA.SDK
{
    public class RuntimeGuild : IDiscordGuild, IProxy<IGuild>
    {
        public IGuild guild = null;

        public string AvatarUrl => guild.IconUrl;

        public ulong Id => guild.Id;

        public string Name => guild.Name;

        public uint ChannelCount => (uint) guild.GetChannelsAsync().GetAwaiter().GetResult().Count;

        public uint VoiceChannelCount => (uint)guild.GetVoiceChannelsAsync().GetAwaiter().GetResult().Count;

        public uint UserCount => (uint)guild.GetUsersAsync().GetAwaiter().GetResult().Count;

        public IDiscordUser Owner => new RuntimeUser(guild.GetOwnerAsync().GetAwaiter().GetResult());

        public List<IDiscordRole> Roles
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

        public async Task<IDiscordMessageChannel> GetDefaultChannel()
        {
            return new RuntimeMessageChannel(await guild.GetDefaultChannelAsync());
        }

        public async Task<IDiscordUser> GetUserAsync(ulong user_id)
        {
            return new RuntimeUser(await guild.GetUserAsync(user_id));
        }

        public async Task<List<IDiscordMessageChannel>> GetChannels()
        {
            List<IGuildChannel> channels = (await guild.GetChannelsAsync()).ToList();
            List<IDiscordMessageChannel> rChannels = new List<IDiscordMessageChannel>();
            foreach (IGuildChannel c in channels)
            {
                rChannels.Add(new RuntimeMessageChannel(c));
            }
            return rChannels;
        }

        public IDiscordRole GetRole(ulong role_id)
        {
            return new RuntimeRole(guild.GetRole(role_id));
        }

        public IGuild ToNativeObject()
        {
            return guild;
        }
    }
}