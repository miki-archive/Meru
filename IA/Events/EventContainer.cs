using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class EventContainer
    {
        public Dictionary<string, CommandEvent> CommandEvents { private set; get; } = new Dictionary<string, CommandEvent>();
        public Dictionary<string, Event> MentionEvents { private set; get; } = new Dictionary<string, Event>();
        public Dictionary<string, UserEvent> JoinServerEvents { private set; get; } = new Dictionary<string, UserEvent>();
        public Dictionary<string, UserEvent> LeaveServerEvents { private set; get; } = new Dictionary<string, UserEvent>();
        
        public Event GetEvent(string name)
        {
            if(CommandEvents.ContainsKey(name))
            {
                return CommandEvents[name];
            }
            if (MentionEvents.ContainsKey(name))
            {
                return MentionEvents[name];
            }
            if (JoinServerEvents.ContainsKey(name))
            {
                return JoinServerEvents[name];
            }
            if (LeaveServerEvents.ContainsKey(name))
            {
                return LeaveServerEvents[name];
            }
            return null;
        }
    }
}
