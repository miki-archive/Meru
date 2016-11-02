using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordUser : IDiscordEntity, IMentionable
    {
        bool IsBot { get; }

        string Username { get; }
        string Discriminator { get; }

        bool HasPermissions(params DiscordChannelPermission[] permissions);
        bool HasPermissions(params DiscordGuildPermission[] permissions);

        Task<DiscordMessage> SendMessage(string text);
        Task SendFile(string path);
    }
}