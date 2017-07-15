using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordGuild : IDiscordEntity
    {
        string AvatarUrl { get; }
        string Name { get; }

        int ChannelCount { get; }
        int VoiceChannelCount { get; }
        int UserCount { get; }

        IDiscordUser Owner { get; }

        List<IDiscordRole> Roles { get; }

        Task<IDiscordUser> GetUserAsync(ulong user_id);

        Task<List<IDiscordMessageChannel>> GetChannels();

        Task<IDiscordMessageChannel> GetDefaultChannel();

        IDiscordRole GetRole(ulong role_id);
    }
}