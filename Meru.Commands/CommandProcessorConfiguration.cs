using Meru.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class CommandProcessorConfiguration
    {
        public bool AutoSearchForCommands { get; set; } = false;

		public bool IgnoreSelf { get; set; } = true;

		public bool IgnoreBots { get; set; } = false;

		public string DefaultPrefix { get; set; } = "";

		public bool DefaultConfigurable { get; set; } = false;

		public DbClient Database { get; set; } = null;

		public bool MentionAsPrefix { get; set; } = true;
	}
}
