using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandAttribute : CommandEntityAttribute
    {
        public CommandAttribute()
        {
            Entity = new Command(Entity);
        }
    }
}
