using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Event
    {
        public string name = "name not set";
        public string[] aliases = new string[0];

        public string description = "description not set for this command!";
        public string[] usage = new string[] { "usage not set!" };
        public string errorMessage = "Something went wrong!";

        public bool canBeOverridenByDefaultPrefix = false;
        public bool canBeDisabled = true;
        public bool defaultEnabled = true;

        public Module module;
        internal EventSystem eventSystem;

        public EventAccessibility accessibility = EventAccessibility.PUBLIC;

        public Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();
        protected Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public int CommandUsed { protected set; get; }

        public Event()
        {
            CommandUsed = 0;
        }

        public Event(Action<Event> info)
        {
            info.Invoke(this);
            CommandUsed = 0;
        }

        public void SetEnabled(IMessage e, bool v)
        {
            if (!canBeDisabled && !v) return;

            ulong id = 0;

            if(!enabled.ContainsKey(e.Channel.Id))
            {
                enabled.Add(id, v);
            }
            else
            {
                enabled[id] = v;
            }
        }
    }
}
