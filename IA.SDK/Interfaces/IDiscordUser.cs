using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordUser : IDiscordEntity, IMentionable
    {
        bool IsBot { get; }

        string Username { get; }
        string Discriminator { get; }

        bool HasPermissions(DiscordChannel channel, params DiscordChannelPermission[] permissions);
        bool HasPermissions(DiscordGuild guild, params DiscordGuildPermission[] permissions);

        Task Ban(DiscordGuild guild);
        Task Kick();

        Task<DiscordMessage> SendMessage(string text);
        Task SendFile(string path);
    }
}