using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Commands
{
    public class CommandProcessor
    {
        public Prefix DefaultPrefix => _prefixes[0];

        public event Func<Command, Task<bool>> OnPreCommandExecute;
        public event Func<Command, Task> OnPreCommandFailure; 

        public event Func<Command, long, bool, Task> OnPostCommandExecute; 

        protected readonly CommandEntity hierarchyRoot = new CommandEntity();
        protected readonly Dictionary<string, Command> cachedCommands = new Dictionary<string, Command>();

        private readonly List<Prefix> _prefixes = new List<Prefix>();

        public CommandProcessor(CommandProcessorConfiguration config)
        {
            _prefixes.Add(new Prefix(config.DefaultPrefix));

            if (config.AutoSearchForCommands)
            {
                CommandSeeker s = new CommandSeeker();
                hierarchyRoot = s.GetCommandsFromAttributeAsync();
                cachedCommands = hierarchyRoot.GetAllEntitiesOf<Command>();
            }
        }

        public async Task MessageReceived(IMessageObject message)
        {
            foreach (Prefix p in _prefixes)
            {
                if (message.Content.StartsWith(p.Value))
                {
                    string command = message.Content
                        .Substring(p.Value.Length)
                        .Split(' ')[0]
                        .ToLower();

                    if (cachedCommands.ContainsKey(command))
                    {
                        Command commandObject = cachedCommands[command];

                        if (OnPreCommandExecute != null)
                        {
                            if (!await OnPreCommandExecute.Invoke(commandObject))
                            {
                                if (OnPostCommandExecute != null)
                                {
                                    await OnPreCommandFailure.Invoke(commandObject);
                                }
                                return;
                            }
                        }

                        Stopwatch timeTaken = Stopwatch.StartNew();
                        try
                        {
                            await cachedCommands[command].ProcessCommand(message);
                            timeTaken.Stop();

                            if (OnPostCommandExecute != null)
                            {
                                await OnPostCommandExecute(commandObject, timeTaken.ElapsedMilliseconds, true);
                            }
                        }
                        catch (Exception e)
                        {
                            timeTaken.Stop();
                            if (OnPostCommandExecute != null)
                            {
                                await OnPostCommandExecute(commandObject, timeTaken.ElapsedMilliseconds, false);
                            }
                        }
                    }
                }
            }
        }
    }
}
