using Meru.SDK;
using Meru.SDK.Interfaces;
using System.Threading.Tasks;

namespace Meru.Events
{
    public class GuildEvent : Event
    {
        public ProcessServerCommand processCommand = async (e) =>
        {
            await (await e.GetDefaultChannel()).SendMessage("This server event has not been set up correctly.");
        };

        public GuildEvent() { }

        public async Task CheckAsync(IDiscordGuild e)
        {
            await Task.Run(() => processCommand(e));
        }
    }
}