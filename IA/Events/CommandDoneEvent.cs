using Meru.SDK.Events;
using Meru.SDK.Interfaces;
using System;
using System.Threading.Tasks;

namespace Meru.Events
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