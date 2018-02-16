using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Meru.Commands.Objects;
using Meru.Common;
using Miki.Common.Log;

namespace Meru.Commands
{
    public class CommandProcessor
    {
        public Prefix DefaultPrefix => prefixes[0];

        public event Func<Command, Task<bool>> OnPreCommandExecute;
        public event Func<Command, Task> OnPreCommandFailure; 

        public event Func<Command, long, bool, Task> OnPostCommandExecute; 

        public readonly CommandEntity hierarchyRoot = new CommandEntity();
        protected readonly Dictionary<string, Command> cachedCommands = new Dictionary<string, Command>();

        private readonly List<Prefix> prefixes = new List<Prefix>();
		private CommandProcessorConfiguration config;

        public CommandProcessor(CommandProcessorConfiguration config)
        {
        	this.config = config;

            if (config.AutoSearchForCommands)
            {
                CommandSeeker s = new CommandSeeker();
                hierarchyRoot = s.GetCommandsFromAttributeAsync();
                cachedCommands = hierarchyRoot.GetAllEntitiesOf<Command>();
            }

			if(config.MentionAsPrefix)
			{
				if(config.DefaultPrefix != "")
				{
					prefixes.Add(new Prefix(config.DefaultPrefix)
					{
						Configurable = config.DefaultConfigurable
					});
				}
				prefixes.Add(new MentionPrefix());
			}
			else
			{
				prefixes.Add(new Prefix(config.DefaultPrefix)
				{
					Configurable = config.DefaultConfigurable
				});
			}
		}

		/// <summary>
		/// Gets your command by querying it through modules.
		/// </summary>
		/// <param name="query">Query, e.g. "moderation.kick" leads to a kick command in the moderation module.</param>
		/// <returns></returns>
		public Command GetCommand(string query)
		{
			return GetCommand(hierarchyRoot, query) as Command;
		}
		private CommandEntity GetCommand(CommandEntity root, string query)
		{
			CommandEntity entity = root.Children
				.FirstOrDefault(x => query.StartsWith(x.Id));

			if(entity is Command command)
			{
				return command;
			}

			int dot = query.IndexOf('.');
			string newQuery = query.Substring(dot == -1 ? 0 : dot)
				.TrimStart('.');

			if(entity == null)
			{
				return null;
			}
			return GetCommand(entity, newQuery);
		}

		/// <summary>
		/// Message receive event, handles commands
		/// </summary>
		/// <param name="message">IMessage object from any IBotProvider</param>
		public async Task MessageReceived(IMessage message)
        {
			try
			{
				if (message.Author.IsBot && config.IgnoreBots && !message.Author.IsSelf)
					return;

				if (message.Author.IsSelf && config.IgnoreSelf)
					return;

				foreach (Prefix p in prefixes)
				{
					if (await p.MatchesAsync(message))
					{
						string prefix = await p.GetPrefixAsync(message);

						string content = message.Content
							.Substring(prefix.Length)
							.TrimStart(' ');

						string command = content
							.Split(' ')[0]
							.ToLowerInvariant();

						string arguments = content
							.Substring(command.Length)
							.TrimStart(' ');

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
							bool success = false;

							CommandEventArgs args = new CommandEventArgs
							{
								Message = message,
								Arguments = arguments,
								Processor = this,
								PrefixUsed = p
							};

							try
							{
								if (await cachedCommands[command].CanBeUsedAsync(message))
								{
									await cachedCommands[command].ProcessAsync(args);
									success = true;
									Log.PrintLine($"[{DateTime.Now.ToShortTimeString()}][cmd]: {message.Author.Name.PadRight(10)} called the command {command.PadRight(10)} in {timeTaken.ElapsedMilliseconds}ms");
								}
							}
							catch (Exception e)
							{
								Log.PrintLine(e.Message + "\n" + e.StackTrace);
							}
							finally
							{
								timeTaken.Stop();
								await OnPostCommandExecute?.Invoke(commandObject, timeTaken.ElapsedMilliseconds, success);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Log.PrintLine(e.Message + e.StackTrace);
			}
		}
    }
}
