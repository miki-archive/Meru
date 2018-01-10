using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandAttribute : CommandEntityAttribute
    {
		/// <summary>
		/// If this command is nested in a MultiCommand. Use this field to set it as the default command.
		/// </summary>
		public bool IsDefault { get => (Entity as Command).IsDefault; set => (Entity as Command).IsDefault = value; }

		public CommandAttribute()
        {
            Entity = new Command(Entity);
        }
    }
}
