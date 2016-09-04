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

        public string description;
        public string[] usage = new string[0];
        public string errorMessage = "Something went wrong!";

        public bool defaultEnabled = true;

        public Module parent;
        public EventSystem origin;

        public EventAccessibility accessibility = EventAccessibility.PUBLIC;
        public EventRange range = EventRange.CHANNEL;

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
            ulong id = 0;
            IGuildChannel g = (e.Channel as IGuildChannel);

            switch (range)
            {
                case EventRange.CHANNEL: id = e.Channel.Id; break;
                case EventRange.SERVER: id = g.Id; break;
                case EventRange.USER: id = e.Author.Id; break;    
            }

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
