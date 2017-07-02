using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class CommandHandlerBuilder
    {
        CommandHandler commandHandler;

        public CommandHandlerBuilder(EventSystem eventSystem)
        {
            commandHandler.eventSystem = eventSystem;
        }

        public CommandHandlerBuilder AddCommand(ICommandEvent cmd)
        {
            foreach(string a in cmd.Aliases)
            {
                commandHandler.aliases.Add(a, cmd.Name.ToLower());
            }
            commandHandler.Commands.Add(cmd.Name.ToLower(), cmd);
            return this;
        }

        public CommandHandlerBuilder SetOwner(ulong owner)
        {
            commandHandler.IsPrivate = true;
            commandHandler.Owner = owner;
            return this;
        }

        public CommandHandlerBuilder DisposeInSeconds(int seconds)
        {
            commandHandler.ShouldBeDisposed = true;
            commandHandler.timeDisposed = DateTime.Now.AddSeconds(seconds);
            return this;
        }

        public CommandHandlerBuilder AddPrefix(string value)
        {
            if (!commandHandler.Prefixes.ContainsKey(value))
            {
                commandHandler.Prefixes.Add(value, new PrefixInstance(value, false, false));
            }
            return this;
        }

        public CommandHandler Build()
        {
            return commandHandler;
        }
    }

    public class CommandHandler
    {
        public bool IsPrivate = false;
        public bool ShouldBeDisposed = false;

        public ulong Owner = 0;

        public DateTime TimeCreated = DateTime.Now;
        internal DateTime timeDisposed;

        internal EventSystem eventSystem;

        public Dictionary<string, PrefixInstance> Prefixes = new Dictionary<string, PrefixInstance>();

        internal Dictionary<string, string> aliases = new Dictionary<string, string>();

        public List<IModule> Modules = new List<IModule>();
        public Dictionary<string, ICommandEvent> Commands = new Dictionary<string, ICommandEvent>();

        public CommandHandler(EventSystem eventSystem)
        {
            this.eventSystem = eventSystem;
        }

        public async Task CheckAsync(IDiscordMessage msg)
        {
            if (IsPrivate)
            {
                if (msg.Author.Id == Owner)
                {
                    foreach (PrefixInstance prefix in Prefixes.Values)
                    {
                        if (await TryRunCommandAsync(msg, prefix))
                        {
                            break;
                        }
                    }
                }
                return;
            }

            foreach (PrefixInstance prefix in Prefixes.Values)
            {
                if (await TryRunCommandAsync(msg, prefix))
                {
                    break;
                }
            }
        }

        public async Task<bool> TryRunCommandAsync(IDiscordMessage msg, PrefixInstance prefix)
        {
            string identifier = await prefix.GetForGuildAsync(msg.Guild.Id);
            string message = msg.Content.ToLower();

            if (msg.Content.StartsWith(identifier))
            {
                string command = message
                    .Substring(identifier.Length)
                    .Split(' ')
                    .First();

                command = (aliases.ContainsKey(command)) ? aliases[command] : command;

                ICommandEvent eventInstance = GetCommandEvent(command);

                if (eventInstance == null)
                {
                    return false;
                }

                if (GetUserAccessibility(msg) >= eventInstance.Accessibility)
                {
                    if (await eventInstance.IsEnabled(msg.Channel.Id) || prefix.ForceCommandExecution && GetUserAccessibility(msg) >= EventAccessibility.DEVELOPERONLY)
                    {
                        Task.Run(() => eventInstance.Check(msg, identifier));
                        return true;
                    }
                }
                else
                {
                    await eventSystem.OnCommandDone(msg, eventInstance, false);
                }
            }
            return false;
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage e)
        {
            if (e.Channel == null) return EventAccessibility.PUBLIC;

            if (eventSystem.Developers.Contains(e.Author.Id)) return EventAccessibility.DEVELOPERONLY;
            if (e.Author.HasPermissions(e.Channel, DiscordGuildPermission.ManageRoles)) return EventAccessibility.ADMINONLY;
            return EventAccessibility.PUBLIC;
        }
        public ICommandEvent GetCommandEvent(string value)
        {
            if(Commands.ContainsKey(value))
            {
                return Commands[value];
            }
            return null;
        }
    }
}
