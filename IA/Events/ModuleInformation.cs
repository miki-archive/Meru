using IA.SDK;
using System.Collections.Generic;

namespace IA.Events
{
    public class ModuleInformation
    {
        internal ModuleInformation()
        {
        }

        public string name = "";
        public bool enabled = true;

        internal EventSystem eventSystem = null;

        public MessageRecievedEventDelegate messageEvent;

        public UserUpdatedEventDelegate userUpdateEvent;

        public GuildUserEventDelegate guildJoinEvent;
        public GuildUserEventDelegate guildLeaveEvent;

        public List<RuntimeCommandEvent> events = new List<RuntimeCommandEvent>();

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}