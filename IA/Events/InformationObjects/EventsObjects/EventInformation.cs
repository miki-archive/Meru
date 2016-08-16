using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public enum EventAccessibility
    {
        PUBLIC,
        ADMINONLY,
        DEVELOPERONLY
    }

    public enum EventRange
    {
        USER,
        CHANNEL,
        SERVER
    }
    
    public class EventInformation
    {
        public string name = "name not set";
        public string[] aliases = new string[0];

        public string description;
        public string[] usage = new string[0];
        public string errorMessage = "Something went wrong!";

        public bool enabled = true;

        public Module parent;
        public EventSystem origin;

        public EventAccessibility accessibility = EventAccessibility.PUBLIC;
        public EventRange range = EventRange.CHANNEL;

        public EventInformation()
        {

        }
        public EventInformation(Action<EventInformation> info)
        {
            info.Invoke(this);
        }
    }
}
