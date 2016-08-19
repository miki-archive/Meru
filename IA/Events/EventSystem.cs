using Discord;
using IA.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IA.Events
{
    public class EventSystem
    {
        public List<ulong> developers = new List<ulong>();

        Dictionary<ulong, string> identifier = new Dictionary<ulong, string>();
        Dictionary<string, string> aliases = new Dictionary<string, string>();

        List<ulong> ignore = new List<ulong>();

        /// <summary>
        /// Variable to check if eventSystem has been defined already.
        /// </summary>
        static BotInformation bot;
        EventContainer events;
        static SQLManager sql;

        public string OverrideIdentifier { private set; get; }

        /// <summary>
        /// Constructor for EventSystem.
        /// </summary>
        /// <param name="botInfo">Optional information for the event system about the bot.</param>
        public EventSystem(Action<BotInformation> botInfo)
        {
            if (bot != null)
            {
                Log.Warning("EventSystem already Defined, terminating...");
                return;
            }

            bot = new BotInformation(botInfo);
            events = new EventContainer();
            sql = new SQLManager(bot.SqlInformation, bot.Identifier);

            sql.TryCreateTable("identifier(id BIGINT, i varchar(255))");

            OverrideIdentifier = bot.Name.ToLower() + ".";

            events.InternalEvents.Add("ia-enabled-db", new Event());
        }

        public void AddCommandEvent(Action<CommandEvent> info)
        {
            CommandEvent newEvent = new CommandEvent();
            info.Invoke(newEvent);
            newEvent.origin = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.CommandEvents.Add(newEvent.name.ToLower(), newEvent);

            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddMentionEvent(Action<CommandEvent> info)
        {
            CommandEvent newEvent = new CommandEvent();
            info.Invoke(newEvent);
            newEvent.origin = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.MentionEvents.Add(newEvent.name.ToLower(), newEvent);

            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddJoinEvent(Action<UserEvent> info)
        {
            UserEvent newEvent = new UserEvent();
            info.Invoke(newEvent);
            newEvent.origin = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.JoinServerEvents.Add(newEvent.name.ToLower(), newEvent);


            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }

        public void AddLeaveEvent(Action<UserEvent> info)
        {
            UserEvent newEvent = new UserEvent();
            info.Invoke(newEvent);
            newEvent.origin = this;
            if (newEvent.aliases.Length > 0)
            {
                foreach (string s in newEvent.aliases)
                {
                    aliases.Add(s, newEvent.name.ToLower());
                }
            }
            events.LeaveServerEvents.Add(newEvent.name.ToLower(), newEvent);


            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
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
        public CommandEvent GetCommandEvent(string id)
        {
            return events.CommandEvents.First(c => { return c.Key == id; }).Value;
        }

        public async void SetIdentifier(MessageEventArgs e, string prefix)
        {
            try
            {
                identifier[e.Server.Id] = prefix;
                await Task.Run(() => sql.SendToSQL(string.Format("UPDATE identifier SET i = \"{0}\" WHERE id = {1};", prefix, e.Server.Id)));
            }
            catch (Exception ex)
            {
                Log.ErrorAt("IABot.EventSystem.SetIdentifier", ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// TODO: add sql support.
        /// </summary>
        /// <param name="serverId">server id to ignore</param>
        public void Ignore(ulong serverId)
        {
            ignore.Add(serverId);
        }

        public async void SetEnabled(string eventName, ulong channelId)
        {
            bool isEnabled = false;
            Event setEvent = GetEvent(eventName);

            if (setEvent != null)
            {
                if (bot.SqlInformation != null)
                {
                    isEnabled = await IsEnabled(setEvent, channelId);
                    await Task.Run(() => sql.SendToSQL($"UPDATE event SET enabled = {isEnabled} WHERE id = {channelId} AND name = '{setEvent.name}';"));
                }
                setEvent.enabled[channelId] = !setEvent.enabled[channelId];
            }
        }
 
        public async Task<string> ListCommands(MessageEventArgs e)
        {
            try
            {
                Dictionary<string, List<string>> moduleEvents = new Dictionary<string, List<string>>();
                moduleEvents.Add("Misc", new List<string>());

                foreach (Event ev in events.CommandEvents.Values)
                {
                    if (await IsEnabled(ev, e.Channel.Id))
                    {
                        if (ev.parent != null)
                        {
                            if (!moduleEvents.ContainsKey(ev.parent.defaultInfo.name))
                            {
                                moduleEvents.Add(ev.parent.defaultInfo.name, new List<string>());
                            }
                            if (GetUserAccessibility(e) >= ev.accessibility)
                            {
                                moduleEvents[ev.parent.defaultInfo.name].Add(ev.name);
                            }
                        }
                        else
                        {
                            moduleEvents["Misc"].Add(ev.name);
                        }
                    }
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
            catch
            {
                return "";
            }
        }

        public async Task OnMessageRecieved(MessageEventArgs e)
        {
            if (e.User.IsBot || e.Channel.IsPrivate || ignore.Contains(e.Server.Id)) return;

            Event internalEvent = events.GetInternalEvent("ia-enabled-db");
            if (!internalEvent.enabled.ContainsKey(e.User.Id))
            {
                Event[] allEvents = events.GetAllEvents();
                foreach(Event ev in allEvents)
                {
                    ulong id = 0;
                    switch (ev.range)
                    {
                        case EventRange.CHANNEL: id = e.Channel.Id; break;
                        case EventRange.SERVER: id = e.Server.Id; break;
                        case EventRange.USER: id = e.User.Id; break;
                    }
                    await IsEnabled(ev, id);
                }
                internalEvent.enabled.Add(e.User.Id, true);
            }

            if (!identifier.ContainsKey(e.Server.Id)) LoadIdentifier(e.Server.Id);

            string message = e.Message.RawText.ToLower();

            if (!message.StartsWith(identifier[e.Server.Id])) return;

            if (await CheckIdentifier(message, identifier[e.Server.Id], e)) return;
            else if (await CheckIdentifier(message, OverrideIdentifier, e)) return;
        }

        public async Task OnMention(MessageEventArgs e)
        {
            foreach (CommandEvent ev in events.MentionEvents.Values)
            {
                await ev.Check(e);
            }
        }

        public async Task OnUserJoin(UserEventArgs e)
        {
            foreach (UserEvent ev in events.JoinServerEvents.Values)
            {
                if (await IsEnabled(ev, e.Server.Id))
                {
                    await ev.Check(e);
                }
            }
        }

        public async Task OnUserLeave(UserEventArgs e)
        {
            foreach (UserEvent ev in events.LeaveServerEvents.Values)
            {
                await ev.Check(e);
            }
        }

        public async Task SimulateMessage(MessageEventArgs e, string message)
        {
            if (e.Channel.IsPrivate || ignore.Contains(e.Server.Id)) return;
            if (!identifier.ContainsKey(e.Server.Id)) LoadIdentifier(e.Server.Id);

            if (!message.StartsWith(identifier[e.Server.Id])) return;

            if (await CheckIdentifier(message, identifier[e.Server.Id], e, false)) return;
            else if (await CheckIdentifier(message, OverrideIdentifier, e, false)) return;
        }

        public EventAccessibility GetUserAccessibility(MessageEventArgs e)
        {
            if (developers.Contains(e.User.Id)) return EventAccessibility.DEVELOPERONLY;
            if (e.User.ServerPermissions.Administrator) return EventAccessibility.ADMINONLY;
            return EventAccessibility.PUBLIC;
        }

        public int CommandsUsed()
        {
            int output = 0;
            try
            {
                foreach (Event e in events.CommandEvents.Values)
                {
                    output += e.CommandUsed;
                }
                return output;
            }
            catch
            {
                return output;
            }
        }
        public int CommandsUsed(string eventName)
        {
            return events.GetEvent(eventName).CommandUsed;
        }

        public void LoadIdentifier(ulong server)
        {
            if (bot.SqlInformation != null)
            {
                string instanceIdentifier = sql.GetIdentifier(server);
                if (instanceIdentifier == "ERROR")
                {
                    sql.SetIdentifier(bot.Identifier, server);
                    identifier.Add(server, bot.Identifier);
                }
                else
                {
                    identifier.Add(server, instanceIdentifier);
                }
            }
            else
            {
                identifier.Add(server, bot.Identifier);
            }
        }
        public string GetIdentifier(ulong server_id)
        {
            if (identifier.ContainsKey(server_id))
            {
                return identifier[server_id];
            }
            else
            {
                return sql.GetIdentifier(server_id);
            }
        }

        async Task<bool> CheckIdentifier(string message, string identifier, MessageEventArgs e, bool doRunCommand = true)
        {
            string command = message.Substring(identifier.Length).Split(' ')[0];

            if (events.CommandEvents.ContainsKey(command))
            {
                if (await IsEnabled(events.CommandEvents[command], e.Channel.Id))
                {
                    if (doRunCommand)
                    {
                        await Task.Run(() => events.CommandEvents[command].Check(e, identifier));

                        return true;
                    }
                }
            }
            else if (aliases.ContainsKey(command))
            {
                if (await IsEnabled(events.CommandEvents[aliases[command]], e.Channel.Id))
                {
                    await Task.Run(() => events.CommandEvents[aliases[command]].Check(e, identifier));
                    return true;
                }
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
                await Task.Run(() => sql.SendToSQL(string.Format("INSERT INTO event(name, id, enabled) VALUES('{0}', {1}, {2});", e.name, id, e.defaultEnabled)));
                e.enabled.Add(id, e.defaultEnabled);
                return e.defaultEnabled;
            }
            e.enabled.Add(id, e.defaultEnabled);
            return (state == 1) ? true : false;
        }
    }
}
