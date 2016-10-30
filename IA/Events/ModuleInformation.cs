using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessMessageEvent(IMessage e); 

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