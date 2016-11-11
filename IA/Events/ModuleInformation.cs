using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessMessageEvent(IMessage e); 

    public class ModuleInformation
    {
        public ModuleInformation()
        {
        }

        public string name = "";
        public bool enabled = true;

        internal EventSystem eventSystem = null;

        public ProcessMessageEvent messageEvent; 

        public GuildEvent guildJoinEvent;
        public GuildEvent guildLeaveEvent;

        public List<CommandEvent> events = new List<CommandEvent>();

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}