using IA.SDK;
using IA.SDK.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public class GameEvent
    {
        private ulong userId = 0;

        private List<CommandEvent> commands = new List<CommandEvent>();
        private EventSystem parent = null;

        public bool CheckAsync(IDiscordMessage _message)
        {
            CommandEvent e = commands.Find(x => { return x.name == _message.Content.Split(' ')[0].ToLower(); });

            if (e != null)
            {
                Task.Run(() =>
                {
                    e.checkCommand(_message, _message.Content.Split(' ')[0], e.aliases);
                });
                return true;
            }
            return false;
        }

        public void End()
        {
            parent.GameEvents.Remove(userId);
        }
    }
}