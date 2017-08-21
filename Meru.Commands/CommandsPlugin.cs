using Meru.Common.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class CommandsPlugin : IPlugin
    {
        public IBot AttachedBot { get; set; }

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

        public async Task StartAsync()
        {
            foreach (CommandProcessor p in Processors)
            {
                AttachedBot.OnMessageReceived += p.MessageReceived;
            }
        }

        public async Task StopAsync()
        {
            foreach (CommandProcessor p in Processors)
            {
                AttachedBot.OnMessageReceived -= p.MessageReceived;
            }
        }
    }
}
