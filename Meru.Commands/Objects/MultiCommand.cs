using Meru.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Commands.Objects
{
    public class MultiCommand : Command
    {
		public Command defaultCommand = null;

		public MultiCommand()
		{
		}
		public MultiCommand(CommandEntity entity) : base(entity)
		{

		}
		public MultiCommand(MultiCommand cmd) : base(cmd)
		{
		}

		public override async Task ProcessAsync(CommandEventArgs msg)
		{
			Command cmd = Children.Where(x => msg.Arguments.TrimStart(' ').StartsWith(x.Id)).FirstOrDefault() as Command;

			if (cmd == null)
			{
				if (defaultCommand != null)
				{
					await defaultCommand?.ProcessAsync(msg);
				}
				return;
			}
			msg.Arguments = msg.Arguments.TrimStart(' ').Substring(cmd.GetFullId().Split('.').Last().Length).TrimStart(' ');
			await cmd.ProcessAsync(msg);
		}
	}
}
