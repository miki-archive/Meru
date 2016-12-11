using Discord;
using IA.SQL;
using IA.SDK;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IA.SDK.Interfaces;

namespace IA.Events
{
    public class EventSystem
    {
        public List<ulong> Developers = new List<ulong>();
        public Dictionary<string, Module> Modules { get; internal set; } = new Dictionary<string, Module>();

        Dictionary<ulong, string> identifier = new Dictionary<ulong, string>();
        Dictionary<string, string> aliases = new Dictionary<string, string>();

        List<ulong> ignore = new List<ulong>();

        /// <summary>
        /// Variable to check if eventSystem has been defined already.
        /// </summary>
        static BotInformation bot;

        internal EventContainer events { private set; get; }
        MySQL sql;

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
            sql = new MySQL(bot.SqlInformation, bot.Identifier);

            MySQL.TryCreateTable("identifier(id BIGINT, i varchar(255))");

            OverrideIdentifier = PrefixValue.Set(bot.Name.ToLower() + ".");
            DefaultIdentifier = bot.Identifier;
        }    

        public async Task OnPrivateMessage(IDiscordMessage arg)
        {
            await Task.CompletedTask;
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

            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddCommandEvent(Action<RuntimeCommandEvent> info)
        {
            RuntimeCommandEvent newEvent = new RuntimeCommandEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            if(newEvent.usage[0] == "usage not set!")
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

            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
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

            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
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


            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
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

            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddContinuousEvent(Action<ContinuousEvent> info)
        {
            ContinuousEvent newEvent = new ContinuousEvent();
            info.Invoke(newEvent);
            newEvent.eventSystem = this;
            events.ContinuousEvents.Add(newEvent.name.ToLower(), newEvent);
            
            MySQL.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public Module CreateModule(Action<ModuleInformation> info)
        {
            Module newModule = new Module(info);
            foreach(Event e in newModule.defaultInfo.events)
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

        public Module GetModuleByName(string name)
        {
            if(Modules.ContainsKey(name.ToLower()))
            {
                return Modules[name.ToLower()];
            }
            Log.Warning($"Could not find Module with name '{name}'");
            return null;
        }

        public async Task SetIdentifierAsync(IGuild e, string prefix)
        {
            if (identifier.ContainsKey(e.Id))
            {
                identifier[e.Id] = prefix.ToLower();
            }
            else
            {
                identifier.Add(e.Id, prefix.ToLower());
            }

            await Task.Run(() => MySQL.Query("UPDATE identifier SET i=?i WHERE id=?id;", null, prefix, e.Id));
        }

        public async Task OnGuildLeave(IGuild e)
        {
            foreach (GuildEvent ev in events.LeaveServerEvents.Values)
            {
                if (await IsEnabled(ev, e.Id))
                {
                    await ev.CheckAsync(e.ToSDKGuild());
                }
            }
        }

        public async Task OnGuildJoin(IGuild e)
        {
            foreach (GuildEvent ev in events.JoinServerEvents.Values)
            {
                if (await IsEnabled(ev, e.Id))
                {
                    await ev.CheckAsync(e.ToSDKGuild());
                }
            }
        }

        public async Task<bool> SetEnabled(string eventName, ulong channelId, bool enabled)
        {        
            Event setEvent = GetEvent(eventName);

            if(!setEvent.canBeDisabled && !enabled|| setEvent == null)
            {
                return false;
            }

            if (setEvent != null)
            {
                if (bot.SqlInformation != null)
                {
                    await MySQL.QueryAsync($"UPDATE event SET enabled=?enabled WHERE id=?id AND name=?name;", null, enabled, channelId, setEvent.name);
                }
                setEvent.enabled[channelId] = enabled;
                return true;
            }
            return false;
        }

        public async Task<string> ListCommands(IDiscordMessage e)
        {
            Dictionary<string, List<string>> moduleEvents = new Dictionary<string, List<string>>();

            moduleEvents.Add("Misc", new List<string>());

            EventAccessibility userEventAccessibility = GetUserAccessibility(e);

            foreach (Event ev in events.CommandEvents.Values)
            {
                if (await IsEnabled(ev, e.Channel.Id) && userEventAccessibility >= ev.accessibility)
                {
                    if (ev.module != null)
                    {
                        if (!moduleEvents.ContainsKey(ev.module.defaultInfo.name))
                        {
                            moduleEvents.Add(ev.module.defaultInfo.name.ToUpper(), new List<string>());
                        }
                        if (GetUserAccessibility(e) >= ev.accessibility)
                        {
                            moduleEvents[ev.module.defaultInfo.name].Add(ev.name);
                        }
                    }
                    else
                    {
                        moduleEvents["Misc"].Add(ev.name);
                    }
                }
            }

            if (moduleEvents["Misc"].Count == 0)
            {
                moduleEvents.Remove("Misc");
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
                output.Remove(output.Length - 2);
                output += "\n\n";
            }
            return output;
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

        public async Task OnCommandDone(IDiscordMessage e, RuntimeCommandEvent commandEvent)
        {
            foreach (CommandDoneEvent ev in events.CommandDoneEvents.Values)
            {
                await ev.processEvent(e, commandEvent);
            }
        }

        public async Task OnMention(IDiscordMessage e)
        {
            foreach (RuntimeCommandEvent ev in events.MentionEvents.Values)
            {
                await ev.Check(e);
            }
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage e)
        {
            if (e.Channel == null) return EventAccessibility.PUBLIC;

            if (Developers.Contains(e.Author.Id)) return EventAccessibility.DEVELOPERONLY;
            if (e.Author.HasPermissions(e.Channel, DiscordGuildPermission.ManageRoles)) return EventAccessibility.ADMINONLY;
            return EventAccessibility.PUBLIC;
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

        public async Task LoadIdentifier(ulong server)
        {
            if (bot.SqlInformation != null)
            {
                await MySQL.QueryAsync("SELECT i FROM identifier WHERE id=?id", async output => 
                {
                    if (output == null)
                    {
                        await MySQL.QueryAsync("INSERT INTO identifier VALUES(?server_id, ?prefix)", null, server, bot.Identifier.Value);
                        identifier.Add(server, bot.Identifier.Value);
                    }
                    else
                    {
                        identifier.Add(server, output["i"].ToString());
                    }
                }, server);
            }
            else
            {
                identifier.Add(server, bot.Identifier.Value);
            }
        }

        public async Task<string> GetIdentifier(ulong server_id)
        {
            if (identifier.ContainsKey(server_id))
            {
                string returnIdentifier = identifier[server_id];
                if(returnIdentifier == "mention")
                {
                    returnIdentifier = Bot.instance.Client.CurrentUser.Mention;
                }

                return returnIdentifier;
            }
            else
            {
                return await MySQL.GetIdentifier(server_id);
            }
        }

        async Task<bool> CheckIdentifier(string message, string identifier, IDiscordMessage e, bool doRunCommand = true)
        {
            if (message.StartsWith(identifier))
            {
                string command = message.Substring(identifier.Length).Split(' ')[0];

                if (events.CommandEvents.ContainsKey(command))
                {
                    if (await IsEnabled(events.CommandEvents[command], e.Channel.Id))
                    {
                        if (doRunCommand)
                        {
                            if (GetUserAccessibility(e) >= events.CommandEvents[command].accessibility)
                            {
                                await Task.Run(() => events.CommandEvents[command].Check(e, identifier));
                                return true;
                            }
                        }
                    }
                }
                else if (aliases.ContainsKey(command))
                {
                    if (await IsEnabled(events.CommandEvents[aliases[command]], e.Channel.Id))
                    {
                        if (GetUserAccessibility(e) >= events.CommandEvents[aliases[command]].accessibility)
                        {
                            await Task.Run(() => events.CommandEvents[aliases[command]].Check(e, identifier));
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        async Task<bool> IsEnabled(Event e, ulong id)
        {
            if (bot.SqlInformation == null) return e.defaultEnabled;

            if (e.enabled.ContainsKey(id))
            {
                return events.CommandEvents[e.name].enabled[id];
            }

            int state = await Task.Run(() => sql.IsEventEnabled(e.name, id));
            if(state == -1)
            {
                await Task.Run(() => MySQL.Query("INSERT INTO event(name, id, enabled) VALUES(?name, ?id, ?enabled);", null, e.name, id, e.defaultEnabled));
                e.enabled.Add(id, e.defaultEnabled);
                return e.defaultEnabled;
            }
            bool actualState = (state == 1) ? true : false;

            e.enabled.Add(id, actualState);
            return actualState;
        }
    }
}
