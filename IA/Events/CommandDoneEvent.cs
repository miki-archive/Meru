using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessCommandDoneEvent(IMessage m, Event e);

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
