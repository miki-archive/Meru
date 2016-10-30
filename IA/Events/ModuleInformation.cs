using System.Collections.Generic;

namespace IA.Events
{
    public class ModuleInformation
    {
        public string name;
        public bool enabled;

        internal EventSystem eventSystem;

        public ProcessMessageEvent messageEvent; 

        public GuildEvent guildJoinEvent;
        public GuildEvent guildLeaveEvent;

        public List<CommandEvent> events = new List<CommandEvent>();
    }
}