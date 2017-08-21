using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class ModuleAttribute : CommandEntityAttribute
    {
        public ModuleAttribute() : base()
        {
            entity = new Module(entity);
        }
    }
}
