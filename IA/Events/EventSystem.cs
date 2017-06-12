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
        public Dictionary<string, IModule> Modules { get; internal set; } = new Dictionary<string, IModule>();
        public Dictionary<ulong, GameEvent> GameEvents { get; internal set; } = new Dictionary<ulong, GameEvent>();

        private Dictionary<string, PrefixInstance> prefixCache = new Dictionary<string, PrefixInstance>();
        private Dictionary<ulong, string> identifierCache = new Dictionary<ulong, string>();
        internal Dictionary<string, string> aliases = new Dictionary<string, string>();

        private List<ulong> ignore = new List<ulong>();

        /// <summary>
        /// Variable to check if eventSystem has been defined already.
        /// </summary>
        public BotInformation bot;

        internal EventContainer events { private set; get; }

        public ExceptionDelegate OnCommandError = async (ex, command, msg) =>
        {
        };

        public string DefaultIdentifier { private set; get; }
        public string OverrideIdentifier { private set; get; }

        /// <summary>
        /// Constructor for EventSystem.
        /// </summary>
        /// <param name="botInfo">Optional information for the event system about the bot.</param>
        public EventSystem(Action<BotInformation> botInfo)
        {
            if (bot != null)
            {
                Log.Warning("EventSystem already defined, terminating...");
                return;
            }

            bot = new BotInformation(botInfo);
            events = new EventContainer();

            OverrideIdentifier = bot.Name.ToLower() + ".";
            DefaultIdentifier = bot.Identifier;
        }

        [Obsolete("pls never use this.")]
        public void AddCommandEvent(Action<RuntimeCommandEvent> info)
        {
            RuntimeCommandEvent newEvent = new RuntimeCommandEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Metadata.usage[0] == "usage not set!")
            {
                newEvent.Metadata.usage[0] = newEvent.Name;
            }
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.CommandEvents.Add(newEvent.Name.ToLower(), newEvent);
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
                    aliases.Add(s, newEvent.Name.ToLower());
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
                    aliases.Add(s, newEvent.Name.ToLower());
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
                    aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.LeaveServerEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

        public void AddMentionEvent(Action<RuntimeCommandEvent> info)
        {
            RuntimeCommandEvent newEvent = new RuntimeCommandEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.Aliases.Length > 0)
            {
                foreach (string s in newEvent.Aliases)
                {
                    aliases.Add(s, newEvent.Name.ToLower());
                }
            }
            events.MentionEvents.Add(newEvent.Name.ToLower(), newEvent);
        }

        public int CommandsUsed()
        {
            int output = 0;
            foreach (Event e in events.CommandEvents.Values)
            {
                output += e.TimesUsed;
            }
            return output;
        }
        public int CommandsUsed(string eventName)
        {
            return events.GetEvent(eventName).TimesUsed;
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
            Modules.Add(newModule.Name, newModule);
            return newModule;
        }

        public ICommandEvent GetCommandEvent(string id)
        {
            if (events.CommandEvents.ContainsKey(id))
            {
                return events.CommandEvents[id];
            }
            return null;
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
            if (Modules.ContainsKey(name.ToLower()))
            {
                return Modules[name.ToLower()];
            }
            Log.Warning($"Could not find Module with name '{name}'");
            return null;
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage e)
        {
            if (e.Channel == null) return EventAccessibility.PUBLIC;

            if (Developers.Contains(e.Author.Id)) return EventAccessibility.DEVELOPERONLY;
            if (e.Author.HasPermissions(e.Channel, DiscordGuildPermission.ManageRoles)) return EventAccessibility.ADMINONLY;
            return EventAccessibility.PUBLIC;
        }

        public async Task<SortedDictionary<string, List<string>>> GetEventNames(IDiscordMessage e)
        {
            SortedDictionary<string, List<string>> moduleEvents = new SortedDictionary<string, List<string>>();

            moduleEvents.Add("MISC", new List<string>());

            EventAccessibility userEventAccessibility = GetUserAccessibility(e);

            foreach (ICommandEvent ev in events.CommandEvents.Values)
            {
                if (await ev.IsEnabled(e.Channel.Id) && userEventAccessibility >= ev.Accessibility)
                {
                    if (ev.Module != null)
                    {
                        if (!moduleEvents.ContainsKey(ev.Module.Name.ToUpper()))
                        {
                            moduleEvents.Add(ev.Module.Name.ToUpper(), new List<string>());
                        }

                        if (GetUserAccessibility(e) >= ev.Accessibility)
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

        public async Task OnCommandDone(IDiscordMessage e, ICommandEvent commandEvent, bool success = true)
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

        public async Task OnGuildLeave(IDiscordGuild e)
        {
            foreach (GuildEvent ev in events.LeaveServerEvents.Values)
            {
                if (await ev.IsEnabled(e.Id))
                {
                    await ev.CheckAsync(e);
                }
            }
        }

        public async Task OnGuildJoin(IDiscordGuild e)
        {
            foreach (GuildEvent ev in events.JoinServerEvents.Values)
            {
                if (await ev.IsEnabled(e.Id))
                {
                    await ev.CheckAsync(e);
                }
            }
        }

        public async Task OnPrivateMessage(IDiscordMessage arg)
        {
            await Task.CompletedTask;
        }

        public async Task OnMention(IDiscordMessage e)
        {
            foreach (RuntimeCommandEvent ev in events.MentionEvents.Values)
            {
                await ev.Check(e);
            }
        }

        public async Task OnMessageRecieved(IDiscordMessage _message)
        {
            if (_message.Author.IsBot)
            {
                return;
            }

            foreach (PrefixInstance prefix in prefixCache.Values)
            {
                if(await TryRunCommandAsync(_message, prefix))
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

                if (GetUserAccessibility(msg) >= events.CommandEvents[command].Accessibility)
                {
                    if (await events.CommandEvents[command].IsEnabled(msg.Channel.Id) || prefix.ForceCommandExecution && GetUserAccessibility(msg) >= EventAccessibility.DEVELOPERONLY)
                    {
                        Task.Run(() => events.CommandEvents[command].Check(msg, identifier)); 
                        return true;
                    }
                }
                else
                {
                    await OnCommandDone(msg, events.CommandEvents[command], false);
                }
            }
            return false;
        }

        /// <summary>
        /// Register a prefix for the bot
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="canBeChanged"></param>
        /// <param name="defaultPrefix"></param>
        /// <param name="forceExecuteCommands"></param>
        /// <returns></returns>
        public PrefixInstance RegisterPrefixInstance(string prefix, bool canBeChanged = true, bool forceExecuteCommands = false)
        {
            PrefixInstance newPrefix = new PrefixInstance(prefix.ToLower(), canBeChanged, forceExecuteCommands);



            prefixCache.Add(prefix, newPrefix);
            return newPrefix;
        }

        public PrefixInstance GetPrefixInstance(string defaultPrefix)
        {
            string prefix = defaultPrefix.ToLower();

            if(prefixCache.ContainsKey(prefix))
            {
                return prefixCache[prefix];
            }
            return null;
        }

        public async Task<string> GetPrefixValueAsync(string defaultPrefix, ulong guildId)
        {
            PrefixInstance instance = prefixCache
                .First(prefix => prefix.Value.IsDefault)
                .Value;

            if(instance == null)
            {
                return "no";
            }

            return await instance.GetForGuildAsync(guildId);
        }
    }
}