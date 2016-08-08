using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class EventContainer
    {
        public Dictionary<string, Event> CommandEvents { private set; get; } = new Dictionary<string, Event>();
        public Dictionary<string, Event> MentionEvents { private set; get; } = new Dictionary<string, Event>();
        public Dictionary<string, Event> JoinServerEvents { private set; get; } = new Dictionary<string, Event>();
        public Dictionary<string, Event> LeaveServerEvents { private set; get; } = new Dictionary<string, Event>();


    }
}
