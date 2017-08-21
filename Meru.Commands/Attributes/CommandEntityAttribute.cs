using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandEntityAttribute : Attribute
    {
        internal CommandEntity entity = new CommandEntity();

        public string Name { get => entity.Id; set => entity.Id = value; }

        internal CommandEntityAttribute()
        {
        }
    }
}
