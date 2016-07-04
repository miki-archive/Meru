using Discord;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class EventSystem
    {
        public static List<ulong> developers = new List<ulong>();

        Dictionary<ulong, string> identifier = new Dictionary<ulong, string>();
        Dictionary<string, Event> events = new Dictionary<string, Event>();
        Dictionary<string, string> aliases = new Dictionary<string, string>();

        List<ulong> ignore = new List<ulong>();

        static BotInformation bot = new BotInformation();

        public string OverrideIdentifier { private set; get; }

        public EventSystem(Action<BotInformation> botInfo)
        {
            botInfo.Invoke(bot);
            MySqlConnection connection = new MySqlConnection(bot.SqlInformation.GetConnectionString());
            try
            {
                SendToSQL("CREATE TABLE identifier(id BIGINT, i varchar(255))");
            }
            catch
            {

            }
            Console.WriteLine("Module 'IA.Events' loaded succesfully!");
            OverrideIdentifier = bot.Name.ToLower() + ".";
        }

        public async void SetPrefix(MessageEventArgs e, string prefix)
        {
            try
            {
                identifier[e.Server.Id] = prefix;
                await Task.Run(() => SendToSQL(string.Format("UPDATE identifier SET i = \"{0}\" WHERE id = {1};", prefix, e.Server.Id)));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        public void Ignore(ulong id)
        {
            ignore.Add(id);
        }

        public async void Toggle(MessageEventArgs e)
        {
            string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0].ToLower();
            string targetcommand = e.Message.RawText.Split(' ')[1].ToLower();

            if(targetcommand == "toggle" || targetcommand == "help" || targetcommand == "info")
            {
                await e.Channel.SendMessage("You can't toggle `" + targetcommand + "`");
                return;
            }

            try
            {
                if (events.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[targetcommand].enabled[e.Channel.Id] = !isEnabled;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events[targetcommand].enabled[e.Channel.Id], e.Channel.Id)));
                    await e.Channel.SendMessage(!isEnabled == true?"Enabled " : "Disabled " + targetcommand + " for this channel!");
                }
                else if (aliases.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[aliases[targetcommand]].enabled[e.Channel.Id] = !isEnabled;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events[aliases[targetcommand]].enabled[e.Channel.Id], e.Channel.Id)));
                    await e.Channel.SendMessage(!isEnabled == true ? "Enabled " : "Disabled " + targetcommand + " for this channel!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        public async void Disable(MessageEventArgs e)
        {
            string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0].ToLower();
            string targetcommand = e.Message.RawText.Split(' ')[1].ToLower();

            if (targetcommand == "toggle" || targetcommand == "help" || targetcommand == "info")
            {
                await e.Channel.SendMessage("You can't toggle 'toggle'");
                return;
            }

            try
            {
                if (targetcommand == "all")
                {
                    foreach (Event ev in events.Values)
                    {
                        if (ev.info.accessibility != EventAccessibility.PUBLIC)
                        {
                            continue;
                        }

                        events[ev.info.name].enabled[e.Channel.Id] = false;
                    }
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, false, e.Channel.Id)));
                    await e.Channel.SendMessage("Disabled everything for this channel!");
                    return;
                }

                if (events.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[targetcommand].enabled[e.Channel.Id] = false;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = false WHERE id = {1} AND name = '{0}';", targetcommand, e.Channel.Id)));
                    await e.Channel.SendMessage("Disabled " + targetcommand + " for this channel!");
                }
                else if (aliases.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[aliases[targetcommand]].enabled[e.Channel.Id] = false;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = {1} WHERE id = {2} AND name = '{0}';", targetcommand, events[aliases[targetcommand]].enabled[e.Channel.Id], e.Channel.Id)));
                    await e.Channel.SendMessage("Disabled " + targetcommand + " for this channel!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
        public async void Enable(MessageEventArgs e)
        {
            string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0].ToLower();
            string targetcommand = e.Message.RawText.Split(' ')[1].ToLower();

            if (targetcommand == "toggle" || targetcommand == "help" || targetcommand == "info")
            {
                await e.Channel.SendMessage("You can't toggle 'toggle'");
                return;
            }

            try
            {
                if (targetcommand == "all")
                {
                    foreach (Event ev in events.Values)
                    {
                        if(ev.info.accessibility != EventAccessibility.PUBLIC)
                        {
                            continue;
                        }

                        events[ev.info.name].enabled[e.Channel.Id] = true;
                        await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = true WHERE id = {1} AND name = '{0}';", targetcommand, e.Channel.Id)));
                    }
                    await e.Channel.SendMessage("Disabled everything for this channel!");
                    return;
                }

                if (events.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[targetcommand].enabled[e.Channel.Id] = true;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = true WHERE id = {1} AND name = '{0}';", targetcommand, e.Channel.Id)));
                    await e.Channel.SendMessage("Enabled " + targetcommand + " for this channel!");
                }
                else if (aliases.ContainsKey(targetcommand))
                {
                    bool isEnabled = await CheckEnabled(events[targetcommand], e.Channel.Id);
                    events[aliases[targetcommand]].enabled[e.Channel.Id] = true;
                    await Task.Run(() => SendToSQL(string.Format("UPDATE event SET enabled = true WHERE id = {1} AND name = '{0}';", targetcommand, e.Channel.Id)));
                    await e.Channel.SendMessage("Enabled " + targetcommand + " for this channel!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        public async Task Check(MessageEventArgs e)
        {
            if(e.Channel.IsPrivate)
            {
                return;
            }
            if(ignore.Contains(e.Server.Id))
            {
                return;
            }


            try
            {
                if (!identifier.ContainsKey(e.Server.Id))
                {
                    string identif = GetIdentifierFromSQL(e.Server.Id);
                    if (identif == "ERROR")
                    {
                        SendToSQL("INSERT INTO identifier VALUES(" + e.Server.Id + ", \">\")");
                        identifier.Add(e.Server.Id, ">");
                    }
                    else
                    {
                        identifier.Add(e.Server.Id, identif);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Something goes wrong inside checking for an identifier");
            }

            if (e.Message.RawText.StartsWith(identifier[e.Server.Id]))
            {
                string command = e.Message.RawText.Substring(identifier[e.Server.Id].Length).Split(' ')[0];

                if (events.ContainsKey(command))
                {
                    if (await CheckEnabled(events[command], e.Channel.Id))
                    {
                        await Task.Run(() => events[command].Check(identifier[e.Server.Id], e));
                    }
                }
                else if (aliases.ContainsKey(command))
                {
                    if (await CheckEnabled(events[aliases[command]], e.Channel.Id))
                    {
                        await Task.Run(() => events[aliases[command]].Check(identifier[e.Server.Id], e));
                    }
                }
            }
            else if (e.Message.RawText.StartsWith(OverrideIdentifier))
            {
                string command = e.Message.RawText.Substring(OverrideIdentifier.Length).Split(' ')[0];

                if (events.ContainsKey(command))
                {
                    if (await CheckEnabled(events[command], e.Channel.Id))
                    {
                        await Task.Run(() => events[command].Check(OverrideIdentifier, e));
                    }
                }
                else if (aliases.ContainsKey(command))
                {
                    if (await CheckEnabled(events[aliases[command]], e.Channel.Id))
                    {
                        await Task.Run(() => events[aliases[command]].Check(OverrideIdentifier, e));
                    }
                }
            }
            else
            {
                if (e.Message.IsMentioningMe())
                {
                    foreach (Event ev in events.Values)
                    {
                        if (ev.info.type == EventType.MENTION)
                        {
                            await Task.Run(() => ev.Check("", e));
                        }
                    }
                }
            }
        }
        public async Task<string> ListCommands(MessageEventArgs e)
        {
            try
            {            
                Dictionary<string, List<string>> moduleEvents = new Dictionary<string, List<string>>();
                moduleEvents.Add("Misc", new List<string>());

                foreach (Event ev in events.Values)
                {
                    if (await CheckEnabled(ev, e.Channel.Id))
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
            catch(Exception ex)
            {
                return "";
            }
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
            foreach(Event e in events.Values)
            {
                output += e.CommandUsed;
            }
            return output;
        }

        public string GetPrefix(ulong server_id)
        {
            try
            {
                return identifier[server_id];
            }
            catch
            {
                Console.WriteLine("Failed getting identifier from " + server_id);
                return "";
            }
        }

        public void AddEvent(Action<EventInformation> info)
        {
            Event newEvent = new Event();
            info.Invoke(newEvent.info);
            events.Add(newEvent.info.name, newEvent);
            if (newEvent.info.aliases.Length > 0)
            {
                foreach (string s in newEvent.info.aliases)
                {
                    aliases.Add(s, newEvent.info.name); 
                }
            }

            try
            {
                SendToSQL("CREATE TABLE event(name VARCHAR(255), id BIGINT, enabled BOOLEAN)");
            }
            catch
            {
            }
        }

        async Task<bool> CheckEnabled(Event e, ulong id)
        {
            if(events[e.info.name].enabled.ContainsKey(id))
            {
                return events[e.info.name].enabled[id];
            }

            int state = await Task.Run(() => GetEnabledFromSQL(e.info.name, id));
            if(state == -1)
            {
                await Task.Run(() => SendToSQL(string.Format("INSERT INTO event(name, id, enabled) VALUES('{0}', {1}, {2});", e.info.name, id, e.info.enabled)));
                return e.info.enabled;
            }
            return (state == 1) ? true : false;
        }

        int GetEnabledFromSQL(string name, ulong id)
        {
            MySqlConnection connection = new MySqlConnection(bot.SqlInformation.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT enabled FROM event WHERE id={1} AND name='{0}'", name, id);
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            string outputName = "ERROR";
            bool output = false;

            while (r.Read())
            {
                output = r.GetBoolean("enabled");
                outputName = name;
                break;
            }
            connection.Close();
            if (outputName != "ERROR")
            {
                return output?1:0;
            }
            return -1;
        }
        string GetIdentifierFromSQL(ulong id)
        {
            MySqlConnection connection = new MySqlConnection(bot.SqlInformation.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT i FROM identifier WHERE id={0}", id);
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            string outputName = "ERROR";
            string output = "";

            while (r.Read())
            {
                output = r.GetString("i");
                outputName = "NOT_ERROR";
                break;
            }
            connection.Close();
            if (outputName != "ERROR")
            {
                return output;
            }
            return "ERROR";
        }

        static void SendToSQL(string sqlcode)
        {
            MySqlConnection connection = new MySqlConnection(bot.SqlInformation.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sqlcode;
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            connection.Close();
        }
    }
}
