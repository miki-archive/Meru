using System.Collections.Generic;
using System.Linq;

namespace IA.Events
{
    internal class EventContainer
    {
        public Dictionary<string, RuntimeCommandEvent> CommandEvents { private set; get; } = new Dictionary<string, RuntimeCommandEvent>();
        public Dictionary<string, CommandDoneEvent> CommandDoneEvents { private set; get; } = new Dictionary<string, CommandDoneEvent>();

        public Dictionary<string, Event> MentionEvents { private set; get; } = new Dictionary<string, Event>();
        public Dictionary<string, Event> ContinuousEvents { private set; get; } = new Dictionary<string, Event>();

        public Dictionary<string, GuildEvent> JoinServerEvents { private set; get; } = new Dictionary<string, GuildEvent>();
        public Dictionary<string, GuildEvent> LeaveServerEvents { private set; get; } = new Dictionary<string, GuildEvent>();

        /// <summary>
        /// I use this to store internal events.
        /// </summary>
        internal Dictionary<string, Event> InternalEvents { private set; get; } = new Dictionary<string, Event>();

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
            if (ContinuousEvents.ContainsKey(name))
            {
                return ContinuousEvents[name];
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
            allEvents.AddRange(ContinuousEvents.Values);
            allEvents.AddRange(JoinServerEvents.Values);
            allEvents.AddRange(LeaveServerEvents.Values);
            return allEvents.ToArray();
        }

        public Dictionary<string, Event> GetAllEventsDictionary()
        {
            Dictionary<string, Event> allEvents = new Dictionary<string, Event>();
            CommandEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            MentionEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            ContinuousEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            JoinServerEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            LeaveServerEvents.ToList().ForEach(x => allEvents.Add(x.Key, x.Value));
            return allEvents;
        }
    }
}