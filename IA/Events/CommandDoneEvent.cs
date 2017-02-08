using IA.SDK.Interfaces;
using System;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessCommandDoneEvent(IDiscordMessage m, Event e);

    public class CommandDoneEvent : Event
    {
        public ProcessCommandDoneEvent processEvent;

        public CommandDoneEvent() : base()
        {
        }

        public CommandDoneEvent(Action<CommandDoneEvent> e)
        {
            e.Invoke(this);
        }
    }
}