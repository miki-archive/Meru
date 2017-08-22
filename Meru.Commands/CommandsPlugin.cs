using Meru.Common.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class CommandsPlugin : BaseExtendablePlugin
    {
        public List<CommandProcessor> Processors = new List<CommandProcessor>();

        public CommandsPlugin(IBot bot)
        {
            AttachedBot = bot;
        }

        public CommandsPlugin(IBot bot, CommandProcessorConfiguration config)
        {
            AttachedBot = bot;

            CommandProcessor processor = new CommandProcessor(config);
            Processors.Add(processor);
        }

        public override async Task StartAsync()
        {
            foreach (CommandProcessor p in Processors)
            {
                AttachedBot.OnMessageReceive += p.MessageReceived;
            }

            await base.StartAsync();
        }

        public override async Task StopAsync()
        {
            foreach (CommandProcessor p in Processors)
            {
                AttachedBot.OnMessageReceive -= p.MessageReceived;
            }

            await base.StopAsync();
        }
    }
}
