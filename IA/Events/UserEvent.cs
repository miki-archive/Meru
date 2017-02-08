using IA.SDK;
using IA.SDK.Interfaces;
using System.Threading.Tasks;

namespace IA.Events
{
    public class GuildEvent : Event
    {
        public ProcessServerCommand processCommand = async (e) =>
        {
            await (await e.GetDefaultChannel()).SendMessage("This server event has not been set up correctly.");
        };

        public GuildEvent()
        {
            CommandUsed = 0;
        }

        public async Task CheckAsync(IDiscordGuild e)
        {
            await Task.Run(() => processCommand(e));
        }
    }
}