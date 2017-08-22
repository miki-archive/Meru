using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class Command : CommandEntity
    {
        public Func<IMessageObject, Task> ProcessCommand;

        public Command()
        {            
        }

        public Command(CommandEntity cmd) : base(cmd)
        {           
        }

        public Command(Command cmd) : base(cmd)
        {
        }
    }
}
