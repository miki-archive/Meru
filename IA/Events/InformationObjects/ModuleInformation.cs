using System.Collections.Generic;

namespace IA.Events
{
    public class ModuleInformation
    {
        public string name;
        public bool enabled;

        public ProcessMessageEvent messageEvent; 

        public GuildEvent guildJoinEvent;
        public GuildEvent guildLeaveEvent;


        List<CommandEvent> events = new List<CommandEvent>();
    }
}