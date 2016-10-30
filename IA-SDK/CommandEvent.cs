using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class CommandEvent : Event
    {
        public int cooldown = 1;

        public GuildPermission[] requiresPermissions = new GuildPermission[0];

        public CheckCommand checkCommand = null;

        public ProcessCommand processCommand = null;

        public CommandEvent(Action<CommandEvent> info) : base(info as Action<Event>) 
        {
            info.Invoke(this);
        }
    }
}
