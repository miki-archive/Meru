using IA.Database;
using IA.SDK;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IA.Events
{
    public class EventSystem
    {
        public List<ulong> Developers = new List<ulong>();
        public Dictionary<string, Module> Modules { get; internal set; } = new Dictionary<string, Module>();
        public Dictionary<ulong, GameEvent> GameEvents { get; internal set; } = new Dictionary<ulong, GameEvent>();

        private Dictionary<ulong, string> identifier = new Dictionary<ulong, string>();
        private Dictionary<string, string> aliases = new Dictionary<string, string>();

        private List<ulong> ignore = new List<ulong>();

        /// <summary>
        /// Variable to check if eventSystem has been defined already.
        /// </summary>
        public BotInformation bot;

        internal EventContainer events { private set; get; }
        private Sql sql;

        public PrefixValue DefaultIdentifier { private set; get; }
        public PrefixValue OverrideIdentifier { private set; get; }

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
            sql = new Sql(bot.SqlInformation, bot.Identifier);

            Sql.TryCreateTable("identifier(id BIGINT, i varchar(255))");

            OverrideIdentifier = PrefixValue.Set(bot.Name.ToLower() + ".");
            DefaultIdentifier = bot.Identifier;
        }

        public void AddCommandEvent(Action<RuntimeCommandEvent> info)
        {
            RuntimeCommandEvent newEvent = new RuntimeCommandEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.usage[0] == "usage not set!")
            {
                newEvent.usage[0] = newEvent.name;
            }
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.CommandEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddCommandDoneEvent(Action<CommandDoneEvent> info)
        {
            CommandDoneEvent newEvent = new CommandDoneEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.CommandDoneEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddContinuousEvent(Action<ContinuousEvent> info)
        {
            ContinuousEvent newEvent = new ContinuousEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            events.ContinuousEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddJoinEvent(Action<GuildEvent> info)
        {
            GuildEvent newEvent = new GuildEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.JoinServerEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddLeaveEvent(Action<GuildEvent> info)
        {
            GuildEvent newEvent = new GuildEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.LeaveServerEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddMentionEvent(Action<RuntimeCommandEvent> info)
        {
            RuntimeCommandEvent newEvent = new RuntimeCommandEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.MentionEvents.Add(newEvent.name.ToLower(), newEvent);

            Sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        private async Task<bool> CheckIdentifier(string message, string identifier, IDiscordMessage e, bool doRunCommand = true)
        {
            if (message.StartsWith(identifier))
            {
                string command = message.Substring(identifier.Length).Split(' ')[0].ToLower();

                if (events.CommandEvents.ContainsKey(command))
                {
                    if (await events.CommandEvents[command].IsEnabled(e.Channel.Id))
                    {
                        if (doRunCommand)
                        {
                            if (GetUserAccessibility(e) >= events.CommandEvents[command].accessibility)
                            {
                                Task.Run(() => events.CommandEvents[command].Check(e, identifier));
                                return true;
                            }
                        }
                    }
                }
                else if (aliases.ContainsKey(command))
                {
                    if (await events.CommandEvents[aliases[command]].IsEnabled(e.Channel.Id))
                    {
                        if (GetUserAccessibility(e) >= events.CommandEvents[aliases[command]].accessibility)
                        {
                            Task.Run(() => events.CommandEvents[aliases[command]].Check(e, identifier));
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public int CommandsUsed()
        {
            int output = 0;
            foreach (Event e in events.CommandEvents.Values)
            {
                output += e.CommandUsed;
            }
            return output;
        }

        public int CommandsUsed(string eventName)
        {
            return events.GetEvent(eventName).CommandUsed;
        }

        public Module CreateModule(Action<ModuleInformation> info)
        {
            Module newModule = new Module(info);
            foreach (Event e in newModule.defaultInfo.events)
            {
                e.eventSystem = this;
                e.module = newModule;
            }
            newModule.defaultInfo.eventSystem = this;
            Modules.Add(newModule.defaultInfo.name, newModule);
            return newModule;
        }

        /// <summary>
        /// Gets only command events as commandevent value
        /// </summary>
        /// <param name="id">event id</param>
        /// <returns>CommandEvent from local database</returns>
        public RuntimeCommandEvent GetCommandEvent(string id)
        {
            if (events.CommandEvents.ContainsKey(id))
            {
                return events.CommandEvents[id];
            }
            return null;
        }

        /// <summary>
        /// Gets event and returns as base value.
        /// </summary>
        /// <param name="id">event id</param>
        /// <returns>Event from local database</returns>
        public Event GetEvent(string id)
        {
            return events.GetEvent(id);
        }

        public async Task<string> GetIdentifier(ulong server_id)
        {
            if (identifier.ContainsKey(server_id))
            {
                string returnIdentifier = identifier[server_id];
                if (returnIdentifier == "mention")
                {
                    // returnIdentifier = Bot.instance.Client.CurrentUser.Mention;
                }

                return returnIdentifier;
            }
            else
            {
                return await Sql.GetIdentifier(server_id);
            }
        }

        public Module GetModuleByName(string name)
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

        public async Task<string> ListCommands(IDiscordMessage e)
        {
            Dictionary<string, List<string>> moduleEvents = new Dictionary<string, List<string>>();

            moduleEvents.Add("MISC", new List<string>());

            EventAccessibility userEventAccessibility = GetUserAccessibility(e);

            foreach (Event ev in events.CommandEvents.Values)
            {
                if (await ev.IsEnabled(e.Channel.Id) && userEventAccessibility >= ev.accessibility)
                {
                    if (ev.module != null)
                    {
                        if (!moduleEvents.ContainsKey(ev.module.defaultInfo.name.ToUpper()))
                        {
                            moduleEvents.Add(ev.module.defaultInfo.name.ToUpper(), new List<string>());
                        }
                        if (GetUserAccessibility(e) >= ev.accessibility)
                        {
                            moduleEvents[ev.module.defaultInfo.name.ToUpper()].Add(ev.name);
                        }
                    }
                    else
                    {
                        moduleEvents["MISC"].Add(ev.name);
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
                list.OrderBy(x =>
                {
                    return x;
                });
            }

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

        public async Task LoadIdentifier(ulong server)
        {
            string tempIdentifier = bot.Identifier.Value;

            if (bot.SqlInformation != null)
            {
                Sql.Query("SELECT i FROM identifier WHERE id=?id", output =>
                {
                    if (output == null)
                    {
                        Sql.Query("INSERT INTO identifier VALUES(?server_id, ?prefix)", null, server, bot.Identifier.Value);
                    }
                    else
                    {
                        tempIdentifier = output.GetString("i");
                    }
                }, server);
            }

            identifier.Add(server, tempIdentifier);
        }

        public async Task OnCommandDone(IDiscordMessage e, RuntimeCommandEvent commandEvent)
        {
            foreach (CommandDoneEvent ev in events.CommandDoneEvents.Values)
            {
                try
                {
                    await ev.processEvent(e, commandEvent);
                }
                catch (Exception ex)
                {
                    Log.ErrorAt($"commanddone@{ev.name}", ex.Message);
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

            if (!identifier.ContainsKey(_message.Guild.Id))
            {
                await LoadIdentifier(_message.Guild.Id);
            }

            string message = _message.Content.ToLower();

            if (await CheckIdentifier(message, identifier[_message.Guild.Id], _message))
            {
                return;
            }

            if (await CheckIdentifier(message, OverrideIdentifier.Value, _message))
            {
                return;
            }
        }

        public async Task SetIdentifierAsync(IDiscordGuild e, string prefix)
        {
            if (identifier.ContainsKey(e.Id))
            {
                identifier[e.Id] = prefix.ToLower();
            }
            else
            {
                identifier.Add(e.Id, prefix.ToLower());
            }

            await Task.Run(() => Sql.Query("UPDATE identifier SET i=?i WHERE id=?id;", null, prefix, e.Id));
        }

        public async Task StartGame(ulong id, GameEvent game)
        {
            if (GameEvents.ContainsKey(id))
            {
                return;
            }
            GameEvents.Add(id, game);
            await Task.CompletedTask;
        }
    }
}