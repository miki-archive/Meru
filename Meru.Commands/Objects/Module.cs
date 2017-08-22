using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class Module : CommandEntity
    {
        public Module()
        {          
        }

        public Module(CommandEntity entity) : base(entity)
        {           
        }

        public Module(Module module) : base(module)
        {         
        }
    }
}
