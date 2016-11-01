using System.IO;
using System.Threading.Tasks;

namespace IA.SDK.Interfaces
{
    public interface IDiscordChannel : IDiscordEntity
    {
        Task SendFileAsync(string path);
        Task SendFileAsync(MemoryStream stream, string extension);

        Task<DiscordMessage> SendMessage(string message);
    }
}