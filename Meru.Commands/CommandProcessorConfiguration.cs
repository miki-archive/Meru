using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandProcessorConfiguration
    {
        public bool AutoSearchForCommands { get; set; } = false;
        public string DefaultPrefix { get; set; } = "";
    }
}
