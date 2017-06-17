using Meru.SDK;
using Meru.SDK.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meru.Events
{
    public class GameEvent
    {
        private ulong userId = 0;

        private List<CommandEvent> commands = new List<CommandEvent>();
        private EventSystem parent = null;

        public bool CheckAsync(IDiscordMessage _message)
        {
            CommandEvent e = commands.Find(x => { return x.Name == _message.Content.Split(' ')[0].ToLower(); });

            if (e != null)
            {
                Task.Run(() =>
                {
                    e.CheckCommand(_message, _message.Content.Split(' ')[0], e.Aliases);
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