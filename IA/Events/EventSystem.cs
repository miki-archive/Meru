using Discord;
using IA.Events.InformationObjects;
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
        }

        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <param name="info">all information for said event</param>
        [Obsolete("use AddCommandEvent instead.", true)]
        public void AddEvent(Action<Event> info)
        {

        }
        public void AddCommandEvent(Action<CommandEventInformation> info)
        {
            CommandEvent newEvent = new CommandEvent();
            info.Invoke(newEvent.info);
            newEvent.info.origin = this;
            if (newEvent.info.aliases.Length > 0)
            {
                foreach (string s in newEvent.info.aliases)
                {
                    aliases.Add(s, newEvent.info.name.ToLower());
                }
            }
            events.CommandEvents.Add(newEvent.info.name.ToLower(), newEvent);

            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
        }
        public void AddJoinEvent(Action<UserEventInformation> info)
        {
            UserEvent newEvent = new UserEvent();
            info.Invoke(newEvent.info);
            newEvent.info.origin = this;
            if (newEvent.info.aliases.Length > 0)
            {
                foreach (string s in newEvent.info.aliases)
                {
                    aliases.Add(s, newEvent.info.name.ToLower());
                }
            }
            events.JoinServerEvents.Add(newEvent.info.name.ToLower(), newEvent);


            sql.TryCreateTable("event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
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

        public async void Toggle(MessageEventArgs e)
        {
            if (e.Message.RawText.Split(' ').Length < 2)
            {
                return;
            }

            string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0].ToLower();
            string targetcommand = e.Message.RawText.Split(' ')[1].ToLower();

            try
            {
                bool isEnabled = false;
                if (events.CommandEvents.ContainsKey(targetcommand))
                {
                    if (bot.SqlInformation != null)
                    {
                        isEnabled = await IsEnabled(events.CommandEvents[targetcommand], e.Channel.Id);
                        Log.Warning("toggle won't be saved due to no sql information");
                    }
                    events.CommandEvents[targetcommand].enabled[e.Channel.Id] = !events.CommandEvents[targetcommand].enabled[e.Channel.Id];
                    if (bot.SqlInformation != null)
                    {
                        await Task.Run(() => sql.SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events.CommandEvents[targetcommand].enabled[e.Channel.Id], e.Channel.Id)));
                        await e.Channel.SendMessage(!isEnabled == true ? "Enabled " : "Disabled " + targetcommand + " for this channel!");
                    }
                }
                else if (aliases.ContainsKey(targetcommand))
                {
                    if (bot.SqlInformation != null)
                    {
                        isEnabled = await IsEnabled(events.CommandEvents[targetcommand], e.Channel.Id);
                    }
                    events.CommandEvents[aliases[targetcommand]].enabled[e.Channel.Id] = !isEnabled;
                    if (bot.SqlInformation != null)
                    {
                        await Task.Run(() => sql.SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events.CommandEvents[aliases[targetcommand]].enabled[e.Channel.Id], e.Channel.Id)));
                        await e.Channel.SendMessage(!isEnabled == true ? "Enabled " : "Disabled " + targetcommand + " for this channel!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
        public async void Toggle(MessageEventArgs e, bool overriddenState)
        {
            if (e.Message.RawText.Split(' ').Length < 2)
            {
                return;
            }

            string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0].ToLower();
            string targetcommand = e.Message.RawText.Split(' ')[1].ToLower();

            try
            {
                bool isEnabled = false;
                if (events.CommandEvents.ContainsKey(targetcommand))
                {
                    if (bot.SqlInformation != null)
                    {
                        isEnabled = await IsEnabled(events.CommandEvents[targetcommand], e.Channel.Id);
                        Log.Warning("toggle won't be saved due to no sql information");
                    }
                    events.CommandEvents[targetcommand].enabled[e.Channel.Id] = overriddenState;
                    if (bot.SqlInformation != null)
                    {
                        await Task.Run(() => sql.SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events.CommandEvents[targetcommand].enabled[e.Channel.Id], e.Channel.Id)));
                        await e.Channel.SendMessage(!isEnabled == true ? "Enabled " : "Disabled " + targetcommand + " for this channel!");
                    }
                }
                else if (aliases.ContainsKey(targetcommand))
                {
                    if (bot.SqlInformation != null)
                    {
                        isEnabled = await IsEnabled(events.CommandEvents[targetcommand], e.Channel.Id);
                    }
                    events.CommandEvents[aliases[targetcommand]].enabled[e.Channel.Id] = overriddenState;
                    if (bot.SqlInformation != null)
                    {
                        await Task.Run(() => sql.SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events.CommandEvents[aliases[targetcommand]].enabled[e.Channel.Id], e.Channel.Id)));
                        await e.Channel.SendMessage(!isEnabled == true ? "Enabled " : "Disabled " + targetcommand + " for this channel!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        public async Task OnMessageRecieved(MessageEventArgs e)
        {
            if (e.Channel.IsPrivate || ignore.Contains(e.Server.Id)) return;
            if (!identifier.ContainsKey(e.Server.Id)) LoadIdentifier(e.Server.Id);

            string message = e.Message.RawText.ToLower();

            if (!message.StartsWith(identifier[e.Server.Id])) return;

            if (await CheckIdentifier(message, identifier[e.Server.Id], e)) return;
            else if (await CheckIdentifier(message, OverrideIdentifier, e)) return;
        }
        public async Task SimulateMessage(MessageEventArgs e, string message)
        {
            if (e.Channel.IsPrivate || ignore.Contains(e.Server.Id)) return;
            if (!identifier.ContainsKey(e.Server.Id)) LoadIdentifier(e.Server.Id);

            if (!message.StartsWith(identifier[e.Server.Id])) return;

            if (await CheckIdentifier(message, identifier[e.Server.Id], e, false)) return;
            else if (await CheckIdentifier(message, OverrideIdentifier, e, false)) return;
        }

        public async Task OnMention(MessageEventArgs e)
        {
            foreach (CommandEvent ev in events.MentionEvents.Values)
            {
                
            }
        }
        public async Task OnUserJoin(UserEventArgs e)
        {
            foreach (UserEvent ev in events.JoinServerEvents.Values)
            {
                //await ev.Check(e);
            }
        }
        public async Task OnUserLeave(UserEventArgs e)
        {
            foreach (UserEvent ev in events.LeaveServerEvents.Values)
            {
                //await ev.Check(e);
            }
        }

        public async Task<bool> CheckIdentifier(string message, string identifier, MessageEventArgs e, bool doRunCommand = true)
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
                        if (ev.info.parent != null)
                        {
                            if (!moduleEvents.ContainsKey(ev.info.parent.defaultInfo.name))
                            {
                                moduleEvents.Add(ev.info.parent.defaultInfo.name, new List<string>());
                            }
                            if (GetUserAccessibility(e) >= ev.info.accessibility)
                            {
                                moduleEvents[ev.info.parent.defaultInfo.name].Add(ev.info.name);
                            }
                        }
                        else
                        {
                            moduleEvents["Misc"].Add(ev.info.name);
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

        public EventAccessibility GetUserAccessibility(MessageEventArgs e)
        {
            try
            {
                if (developers.Contains(e.User.Id)) return EventAccessibility.DEVELOPERONLY;
                if (e.User.ServerPermissions.Administrator) return EventAccessibility.ADMINONLY;
                return EventAccessibility.PUBLIC;
            }
            catch
            {
                return EventAccessibility.DEVELOPERONLY;
            }
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

        async Task<bool> IsEnabled(Event e, ulong id)
        {
            if (bot.SqlInformation == null) return true;

            if (events.CommandEvents[e.info.name].enabled.ContainsKey(id))
            {
                return events.CommandEvents[e.info.name].enabled[id];
            }

            int state = await Task.Run(() => sql.IsEventEnabled(e.info.name, id));
            if(state == -1)
            {
                await Task.Run(() => sql.SendToSQL(string.Format("INSERT INTO event(name, id, enabled) VALUES('{0}', {1}, {2});", e.info.name, id, e.info.enabled)));
                e.enabled.Add(id, e.info.enabled);
                return e.info.enabled;
            }
            e.enabled.Add(id, e.info.enabled);
            return (state == 1) ? true : false;
        }
    }
}
