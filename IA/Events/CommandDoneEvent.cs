using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessCommandDoneEvent(IDiscordMessage m, ICommandEvent e, bool success);

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