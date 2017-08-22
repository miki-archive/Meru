using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class ModuleAttribute : CommandEntityAttribute
    {
        public ModuleAttribute()
        {
            Entity = new Module(Entity);
        }
    }
}
