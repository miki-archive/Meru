using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class CommandEvent:Event
    {
        public CommandEvent(EventInformation info) : base(info)
        {
        }
    }
}
