using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordUser : IDiscordEntity, IMentionable
    {
        bool IsBot { get; }

        string Username { get; }
        string Discriminator { get; }

        Task<DiscordChannelPermission> GetPermissions(IDiscordChannel channel);

        Task<DiscordMessage> SendMessage(string text);
        Task SendFile(string path);
    }
}