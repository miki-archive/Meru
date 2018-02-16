using Meru.Common.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class CommandsPlugin : BaseExtendablePlugin
    {
        private List<CommandProcessor> processors = new List<CommandProcessor>();

        public CommandsPlugin(IBot bot)
        {
            AttachedBot = bot;
        }
        public CommandsPlugin(IBot bot, CommandProcessorConfiguration config)
        {
            AttachedBot = bot;

            CommandProcessor processor = new CommandProcessor(config);
            processors.Add(processor);
        }

		/// <summary>
		/// Starts all instances of the Plugin
		/// </summary>
		public override async Task StartAsync()
        {
            foreach (CommandProcessor p in processors)
            {
                AttachedBot.OnMessageReceive += p.MessageReceived;
            }
            await base.StartAsync();
        }

		/// <summary>
		/// Stops all instances of the Plugin
		/// </summary>
        public override async Task StopAsync()
        {
            foreach (CommandProcessor p in processors)
            {
                AttachedBot.OnMessageReceive -= p.MessageReceived;
            }
            await base.StopAsync();
        }
    }
}
