using Meru.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands.Objects
{
    public class CommandEventArgs
    {
		public IMessage Message { get; set; }
		public string Arguments { get; set; }
		public CommandProcessor Processor { get; set; }
		public Prefix PrefixUsed { get; set; }
    }
}
