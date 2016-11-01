using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordMessage : IDiscordEntity
    {
        IDiscordUser Author { get; }
        IDiscordChannel Channel { get; }
        DiscordGuild Guild { get; }

        string Content { get; }
        DateTimeOffset Timestamp { get; }

        IReadOnlyCollection<ulong> MentionedUserIds { get; }
        IReadOnlyCollection<ulong> MentionedRoleIds { get; }



        Task DeleteAsync();
        Task ModifyAsync(string message);
        Task PinAsync();
        Task UnpinAsync();
    }
}
