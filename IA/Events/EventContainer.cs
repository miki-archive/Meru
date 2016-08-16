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

        /// <summary>
        /// I use this to store internal events.
        /// </summary>
        public Dictionary<string, Event> InternalEvents { private set; get; } = new Dictionary<string, Event>();

        public Event GetEvent(string name)
        {
            if (CommandEvents.ContainsKey(name))
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
        public Event GetInternalEvent(string name)
        {
            return InternalEvents[name];
        }
        public Event[] GetAllEvents()
        {
            List<Event> allEvents = new List<Event>();
            allEvents.AddRange(CommandEvents.Values);
            allEvents.AddRange(MentionEvents.Values);
            allEvents.AddRange(JoinServerEvents.Values);
            allEvents.AddRange(LeaveServerEvents.Values);
            return allEvents.ToArray();
        }
    }
}
