using IA.Database;
using IA.SDK;
using IA.SDK.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Event : IEvent
    {
        public string Name { get; set; } = "$command-not-named";
        public string[] Aliases { get; set; } = new string[] { };

        public EventAccessibility Accessibility { get; set; } = EventAccessibility.PUBLIC;
        public EventMetadata Metadata { get; set; } = new EventMetadata();

        public bool OverridableByDefaultPrefix { get; set; } = false;
        public bool CanBeDisabled { get; set; } = false;
        public bool DefaultEnabled { get; set; } = true;
  
        public IModule Module { get; set; }

        public int TimesUsed { get; set; } = 0;

        internal EventSystem eventSystem;

        public Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();
        protected Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public Event() { }
        public Event(IEvent eventObject)
        {
            Name = eventObject.Name;
            Aliases = eventObject.Aliases;
            Accessibility = eventObject.Accessibility;
            Metadata = eventObject.Metadata;
            OverridableByDefaultPrefix = eventObject.OverridableByDefaultPrefix;
            CanBeDisabled = eventObject.CanBeDisabled;
            DefaultEnabled = eventObject.DefaultEnabled;
            Module = eventObject.Module;
            TimesUsed = eventObject.TimesUsed;
        }
        public Event(Action<Event> info)
        {
            info.Invoke(this);
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
                await Sql.QueryAsync($"UPDATE event SET enabled=?enabled WHERE id=?id AND name=?name;", null, enabled, serverid, Name);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (Module != null)
            {
                if (!await Module.IsEnabled(id)) return false;
            }

            if (eventSystem.bot.SqlInformation == null)
            {
                return DefaultEnabled;
            }

            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            int state = IsEventEnabled(id);
            if (state == -1)
            {
                await Sql.QueryAsync("INSERT INTO event(name, id, enabled) VALUES(?name, ?id, ?enabled);", null, Name, id, DefaultEnabled);
                enabled.Add(id, DefaultEnabled);
                return DefaultEnabled;
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
            command.CommandText = $"SELECT enabled FROM event WHERE id=\"{serverid}\" AND name=\"{Name}\"";

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