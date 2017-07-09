using Discord;
using Discord.WebSocket;
using IA.Models;
using IA.Models.Context;
using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace IA.Events
{
    public class EventSystem
    {
        public delegate Task ExceptionDelegate(Exception ex, ICommandEvent command, IDiscordMessage message);

        public List<ulong> Developers = new List<ulong>();

        public CommandHandler CommandHandler;
        List<CommandHandler> commandHandlers = new List<CommandHandler>();
        Dictionary<Tuple<ulong, ulong>, CommandHandler> privateCommandHandlers = new Dictionary<Tuple<ulong, ulong>, CommandHandler>();

        public Dictionary<string, IModule> Modules => CommandHandler.Modules;
        public Dictionary<string, ICommandEvent> Commands => CommandHandler.Commands;

        private List<ulong> ignore = new List<ulong>();

        /// <summary>
        /// Variable to check if eventSystem has been defined already.
        /// </summary>
        public Bot bot = null;

        internal EventContainer events { private set; get; }

        public ExceptionDelegate OnCommandError = async (ex, command, msg) => { };

        /// <summary>
        /// Constructor for EventSystem.
        /// </summary>
        /// <param name="botInfo">Optional information for the event system about the bot.</param>
        public EventSystem(Bot bot)
        {
            if (this.bot != null)
            {
                Log.Warning("EventSystem already defined, terminating...");
                return;
            }

            this.bot = bot;
            events = new EventContainer();
            CommandHandler = new CommandHandler(this);

            bot.Client.MessageReceived += InternalMessageReceived;
            bot.Client.JoinedGuild += InternalJoinedGuild;
            bot.Client.LeftGuild += InternalLeftGuild;
        }

        public void AddCommandDoneEvent(Action<CommandDoneEvent> info)
        {
            CommandDoneEvent newEvent = new CommandDoneEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    CommandHandler.aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.CommandDoneEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

        public void AddContinuousEvent(Action<ContinuousEvent> info)
        {
            ContinuousEvent newEvent = new ContinuousEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            events.ContinuousEvents.Add(newEvent.Name.ToLower(), newEvent);

        }

        public void AddJoinEvent(Action<GuildEvent> info)
        {
            GuildEvent newEvent = new GuildEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    CommandHandler.aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.JoinServerEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

        public void AddLeaveEvent(Action<GuildEvent> info)
        {
            GuildEvent newEvent = new GuildEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    CommandHandler.aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.LeaveServerEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

        public int CommandsUsed()
        {
            int output = 0;
            foreach (ICommandEvent e in CommandHandler.Commands.Values)
            {
                output += e.TimesUsed;
            }
            return output;
        }
        public int CommandsUsed(string eventName)
        {
            return CommandHandler.GetCommandEvent(eventName).TimesUsed;
        }

        public RuntimeModule CreateModule(Action<IModule> info)
        {
            RuntimeModule newModule = new RuntimeModule(info);
            foreach (Event e in newModule.Events)
            {
                e.eventSystem = this;
                e.Module = newModule;
            }
            newModule.EventSystem = this;
            CommandHandler.AddModule(newModule);
            return newModule;
        }

        public IEvent GetEvent(string id)
        {
            return events.GetEvent(id);
        }

        public async Task<string> GetIdentifier(ulong guildId, PrefixInstance prefix)
        {
            using (var context = new IAContext())
            {
                Identifier i = await context.Identifiers.FindAsync(guildId);
                if (i == null)
                {
                    i = context.Identifiers.Add(new Identifier() { GuildId = guildId.ToDbLong(), Value = prefix.DefaultValue });
                    await context.SaveChangesAsync();
                }
                return i.Value;
            }
        }

        public IModule GetModuleByName(string name)
        {
            if (CommandHandler.Modules.ContainsKey(name.ToLower()))
            {
                return CommandHandler.Modules[name.ToLower()];
            }
            Log.Warning($"Could not find Module with name '{name}'");
            return null;
        }

        public async Task<SortedDictionary<string, List<string>>> GetEventNames(IDiscordMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = new SortedDictionary<string, List<string>>();

            moduleEvents.Add("MISC", new List<string>());

            EventAccessibility userEventAccessibility = CommandHandler.GetUserAccessibility(e);

            foreach (ICommandEvent ev in CommandHandler.Commands.Values)
            {
                if (await ev.IsEnabled(e.Channel.Id) && userEventAccessibility >= ev.Accessibility)
                {
                    if (ev.Module != null)
                    {
                        if (!moduleEvents.ContainsKey(ev.Module.Name.ToUpper()))
                        {
                            moduleEvents.Add(ev.Module.Name.ToUpper(), new List<string>());
                        }

                        if (CommandHandler.GetUserAccessibility(e) >= ev.Accessibility)
                        {
                            moduleEvents[ev.Module.Name.ToUpper()].Add(ev.Name);
                        }
                    }
                    else
                    {
                        moduleEvents["MISC"].Add(ev.Name);
                    }
                }
            }

            if (moduleEvents["MISC"].Count == 0)
            {
                moduleEvents.Remove("MISC");
            }

            moduleEvents.OrderBy(i => { return i.Key; });

            foreach (List<string> list in moduleEvents.Values)
            {
                list.Sort((x, y) => x.CompareTo(y));
            }

            return moduleEvents;
        }

        internal void DisposeCommandHandler(CommandHandler commandHandler)
        {
            commandHandlers.Remove(commandHandler);
        }

        internal void DisposePrivateCommandHandler(Tuple<ulong, ulong> key)
        {
            privateCommandHandlers.Remove(key);

        }
        internal void DisposePrivateCommandHandler(IDiscordMessage msg)
        {
            DisposePrivateCommandHandler(new Tuple<ulong, ulong>(msg.Author.Id, msg.Channel.Id));
        }

        public async Task<string> ListCommands(IDiscordMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = await GetEventNames(e);

            string output = "";
            foreach (KeyValuePair<string, List<string>> items in moduleEvents)
            {
                output += "**" + items.Key + "**\n";
                for (int i = 0; i < items.Value.Count; i++)
                {
                    output += items.Value[i] + ", ";
                }
                output = output.Remove(output.Length - 2);
                output += "\n\n";
            }
            return output;
        }
        public async Task<IDiscordEmbed> ListCommandsInEmbed(IDiscordMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = await GetEventNames(e);

            IDiscordEmbed embed = new RuntimeEmbed(new Discord.EmbedBuilder());

            foreach (KeyValuePair<string, List<string>> items in moduleEvents)
            {
                embed.AddField(f =>
                {
                    f.Name = items.Key;
                    f.Value = "```" + string.Join(", ", items.Value) + "```";
                    f.IsInline = true;
                });
            }
            return embed;
        }

        public PrefixInstance RegisterPrefixInstance(string prefix, bool canBeChanged = true, bool forceExecuteCommands = false)
        {
            PrefixInstance newPrefix = new PrefixInstance(prefix.ToLower(), canBeChanged, forceExecuteCommands);
            CommandHandler.Prefixes.Add(prefix, newPrefix);
            return newPrefix;
        }

        public PrefixInstance GetPrefixInstance(string defaultPrefix)
        {
            string prefix = defaultPrefix.ToLower();

            if(CommandHandler.Prefixes.ContainsKey(prefix))
            {
                return CommandHandler.Prefixes[prefix];
            }
            return null;
        }

        public async Task<string> GetPrefixValueAsync(string defaultPrefix, ulong guildId)
        {
            PrefixInstance instance = CommandHandler.Prefixes
                .First(prefix => prefix.Value.IsDefault)
                .Value;

            if(instance == null)
            {
                return "no";
            }

            return await instance.GetForGuildAsync(guildId);
        }

        #region events
        internal async Task OnCommandDone(IDiscordMessage e, ICommandEvent commandEvent, bool success = true)
        {
            foreach (CommandDoneEvent ev in events.CommandDoneEvents.Values)
            {
                try
                {
                    await ev.processEvent(e, commandEvent, success);
                }
                catch (Exception ex)
                {
                    Log.ErrorAt($"commanddone@{ev.Name}", ex.Message);
                }
            }
        }

        private async Task OnGuildLeave(IDiscordGuild e)
        {
            foreach (GuildEvent ev in events.LeaveServerEvents.Values)
            {
                if (await ev.IsEnabled(e.Id))
                {
                    await ev.CheckAsync(e);
                }
            }
        }
        private async Task OnGuildJoin(IDiscordGuild e)
        {
            foreach (GuildEvent ev in events.JoinServerEvents.Values)
            {
                if (await ev.IsEnabled(e.Id))
                {
                    await ev.CheckAsync(e);
                }
            }
        }
        private async Task OnPrivateMessage(IDiscordMessage arg)
        {
            await Task.CompletedTask;
        }
        private async Task OnMention(IDiscordMessage e)
        {
            foreach (RuntimeCommandEvent ev in events.MentionEvents.Values)
            {
                await ev.Check(e, null);
            }
        }
        private async Task OnMessageRecieved(IDiscordMessage _message)
        {
            if (_message.Author.IsBot)
            {
                return;
            }

            await CommandHandler.CheckAsync(_message);

            foreach (CommandHandler c in commandHandlers)
            {
                if(c.ShouldBeDisposed && c.ShouldDispose())
                {
                    commandHandlers.Remove(c);
                }

                await c.CheckAsync(_message);
            }

            Tuple<ulong, ulong> privateKey = new Tuple<ulong, ulong>(_message.Author.Id, _message.Channel.Id);

            if(privateCommandHandlers.ContainsKey(privateKey))
            {
                    if (privateCommandHandlers[privateKey].ShouldBeDisposed && privateCommandHandlers[privateKey].ShouldDispose())
                    {
                        privateCommandHandlers.Remove(privateKey);
                    }
                    else
                    {
                        await privateCommandHandlers[privateKey].CheckAsync(_message);
                    }
            }
        }

        private void AddPrivateCommandHandler(Tuple<ulong, ulong> key, CommandHandler value)
        {
            if (privateCommandHandlers.ContainsKey(key)) return;

            privateCommandHandlers.Add(key, value);
        }
        public void AddPrivateCommandHandler(IDiscordMessage msg, CommandHandler cHandler)
        {
            AddPrivateCommandHandler(new Tuple<ulong, ulong>(msg.Author.Id, msg.Channel.Id), cHandler);
        }

        private async Task InternalMessageReceived(SocketMessage message)
        {
            try
            {
                IGuild g = (((message as IUserMessage)?.Channel) as IGuildChannel)?.Guild;
                DiscordSocketClient client;
                RuntimeMessage r;

                if (g != null)
                {
                    client = bot.Client.GetShardFor(g);
                    r = new RuntimeMessage(message, client);
                    await OnMessageRecieved(r);
                }
                else
                {
                    client = bot.Client.GetShard(0);
                    r = new RuntimeMessage(message, client);
                    await OnPrivateMessage(r);
                }
          
                if (r.MentionedUserIds.Contains(Bot.instance.Client.CurrentUser.Id))
                {
                    await OnMention(r);
                }
            }
            catch (Exception e)
            {
                Log.ErrorAt("messagerecieved", e.ToString());
            }
        }
        private async Task InternalJoinedGuild(IGuild arg)
        {
            RuntimeGuild g = new RuntimeGuild(arg);
            await OnGuildJoin(g);
        }
        private async Task InternalLeftGuild(IGuild arg)
        {
            RuntimeGuild g = new RuntimeGuild(arg);
            await OnGuildLeave(g);
        }
        #endregion
    }
}