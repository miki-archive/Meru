using Discord;
using IA.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate EventInformation CommandInfoDelegate(EventInformation info);

    class Event
    {
        public Dictionary<ulong, EventInformation> info = new Dictionary<ulong, EventInformation>();
        public EventInformation baseEventInformation;

        public Event(EventInformation info)
        {
            baseEventInformation = info;
            try
            {
                SQL.Query("CREATE TABLE event_information_" + info.name + "(id BIGINT, name VARCHAR(256), enabled BOOLEAN)");
            }
            catch
            {

            }
        }

        public void SetActive(ulong id, bool value)
        {
            info[id].enabled = value;
        }

        public async void Trigger(MessageEventArgs e)
        {
            if (!info.ContainsKey(e.Channel.Id))
            {
                Load(e.Channel.Id);
            }
            try
            {
                if (info[e.Channel.Id].enabled)
                {
                    if (baseEventInformation.developerOnly && e.User.Id != Global.DeveloperId) return;
                    if ((baseEventInformation.adminOnly && !e.User.ServerPermissions.Administrator)) return;
                    await Task.Run(() => baseEventInformation.processCommand(e));
                    Log.Message("command executed!");
                }
                else

                {
                    await e.Channel.SendMessage(":no_entry_sign: This command is not enabled on this server!");
                }
            }
            catch (Exception ex)
            {
                Log.ErrorAt(info[e.Channel.Id].name + "@Event", ex.Message);
            }
        }

        public void Load(ulong id)
        {
            EventInformation eventInfo = GetInfoFromSQL(baseEventInformation.name, id);
            if (eventInfo != null)
            {
                info.Add(id, eventInfo);
            }
            else
            {
                SQL.Query(string.Format("INSERT INTO event_information_{0} (id, enabled) VALUES ({1}, {2})", baseEventInformation.name, id, true));
                eventInfo = GetInfoFromSQL(baseEventInformation.name, id);
                Log.Notice("enabled: " + eventInfo.enabled);
                info.Add(id, eventInfo);
                if(eventInfo == null)
                {
                    throw new Exception("cannot load sql data");
                }
            }
        }

        public bool IsLoaded(ulong id)
        {
            return info.ContainsKey(id);
        }

        public Task<EventInformation> GetInfoFromSQL(string name, ulong id)
        {

            MySqlCommand command = SQL.connection.CreateCommand();
            command.CommandText = string.Format("SELECT enabled FROM event_information_{0} WHERE id={1} ", name, id);
            SQL.connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            Task<EventInformation> eventOutput;
            eventOutput.name = "ERROR";
            while (r.Read())
            {
                eventOutput.enabled = r.GetBoolean("enabled");
                eventOutput.name = name;
                break;
            }
            SQL.connection.Close();
            if(eventOutput.name != "ERROR")
            {
                return eventOutput;
            }
            return null;
        }
    }
}
