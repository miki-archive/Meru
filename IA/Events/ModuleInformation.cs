using Discord;
using IA.SDK;
using IA.SDK.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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