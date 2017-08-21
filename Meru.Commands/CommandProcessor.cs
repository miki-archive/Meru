using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class CommandProcessor
    {
        public Prefix DefaultPrefix => prefixes[0];

        private readonly CommandEntity hierarchyRoot = new CommandEntity();
        private readonly Dictionary<string, Command> cachedCommands = new Dictionary<string, Command>();

        private readonly List<Prefix> prefixes = new List<Prefix>();

        public CommandProcessor(CommandProcessorConfiguration config)
        {
            prefixes.Add(new Prefix(config.DefaultPrefix));

            if (config.AutoSearchForCommands)
            {
                CommandSeeker s = new CommandSeeker();
                hierarchyRoot = s.GetCommandsFromAttributeAsync();
                cachedCommands = hierarchyRoot.GetAllEntitiesOf<Command>();
            }
        }

        public async Task MessageReceived(IMessageObject message)
        {
            foreach (Prefix p in prefixes)
            {
                if (message.Content.StartsWith(p.Value))
                {
                    string command = message.Content
                        .Substring(p.Value.Length)
                        .Split(' ')[0]
                        .ToLower();

                    await cachedCommands[command].ProcessCommand(message);
                }
            }
        }
    }
}
