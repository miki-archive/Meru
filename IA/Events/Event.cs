using Discord;
using IA.SDK;
using IA.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Event
    {
        public string name = "name not set";
        public string[] aliases = new string[0];

        public string description = "description not set for this command!";
        public string[] usage = new string[] { "usage not set!" };
        public string errorMessage = "Something went wrong!";

        public bool canBeOverridenByDefaultPrefix = false;
        public bool canBeDisabled = true;
        public bool defaultEnabled = true;

        public Module module;
        internal EventSystem eventSystem;

        public EventAccessibility accessibility = EventAccessibility.PUBLIC;

        public Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();
        protected Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public int CommandUsed { protected set; get; }

        public Event()
        {
            CommandUsed = 0;
        }

        public Event(Action<Event> info)
        {
            info.Invoke(this);
            CommandUsed = 0;
        }


        public async Task SetEnabled(ulong serverid, bool enabled)
        {
            if (eventSystem.bot.SqlInformation != null)
            {
                if (this.enabled.ContainsKey(serverid))
                {
                    this.enabled[serverid] = enabled;
                }
                else
                {
                    this.enabled.Add(serverid, enabled);
                }
                await MySQL.QueryAsync($"UPDATE event SET enabled=?enabled WHERE id=?id AND name=?name;", null, enabled, serverid, name);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (module != null)
            {
                if (!await module.IsEnabled(id)) return false;
            }

            if (eventSystem.bot.SqlInformation == null)
            {
                return defaultEnabled;
            }

            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            int state = IsEventEnabled(id);
            if (state == -1)
            {
                await MySQL.QueryAsync("INSERT INTO event(name, id, enabled) VALUES(?name, ?id, ?enabled);", null, name, id, defaultEnabled);
                enabled.Add(id, defaultEnabled);
                return defaultEnabled;
            }
            bool actualState = (state == 1) ? true : false;

            enabled.Add(id, actualState);
            return actualState;
        }

        // TODO: Query this.
        public int IsEventEnabled(ulong serverid)
        {
            if (eventSystem.bot.SqlInformation == null) return 1;

            MySqlConnection connection = new MySqlConnection(eventSystem.bot.SqlInformation.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT enabled FROM event WHERE id=\"{serverid}\" AND name=\"{name}\"";

            connection.Open();
            MySqlDataReader r = command.ExecuteReader();

            bool output = false;
            string check = "";

            while (r.Read())
            {
                output = r.GetBoolean(0);
                check = "ok";
                break;
            }
            connection.Close();

            if (check == "")
            {
                return -1;
            }
            return output ? 1 : 0;
        }
    }
}
