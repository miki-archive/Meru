using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandEntityAttribute : Attribute
    {
        internal CommandEntity Entity = new CommandEntity();

        public string Name { get => Entity.Id; set => Entity.Id = value; }

        internal CommandEntityAttribute()
        {
        }
    }
}
