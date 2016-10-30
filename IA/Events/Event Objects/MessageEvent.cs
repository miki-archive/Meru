using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task ProcessMessageEvent(IMessage m, Event e);

    public class MessageEvent : Event
    {
        public ProcessMessageEvent processCommand = async (message, base_event) =>
        {
            await Task.CompletedTask;
        };

        public MessageEvent()
        {
            CommandUsed = 0;
        }

        public async Task Check(IMessage e)
        {
            await Task.Run(() => processCommand(e, this));
        }
    }
}
