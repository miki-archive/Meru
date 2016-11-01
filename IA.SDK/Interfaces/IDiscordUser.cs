

using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordUser : IDiscordEntity, IMentionable
    {
        string Username { get; }
        string Discriminator { get; }

        Task<DiscordMessage> SendMessage(string text);
        Task SendFile(string path);
    }
}