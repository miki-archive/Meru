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

        public Event(EventInformation info)
        {
            this.info.Add(0, info);
            try
            {
                SQL.Query("CREATE TABLE event_information_" + info.name + " VALUES(id BIGINT name TEXT enabled BOOLEAN)");
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
                EventInformation ev = GetInfoFromSQL(info[0].name, e.Channel.Id);
                if (ev.name != "")
                {
                    info.Add(e.Channel.Id, ev);
                }
            }

            if (info[e.Channel.Id].enabled)
            {
                try
                {
                    if (info[e.Channel.Id].developerOnly && e.User.Id != Global.DeveloperId) return;
                    if ((info[e.Channel.Id].adminOnly && !e.User.ServerPermissions.Administrator)) return;
                    await Task.Run(() => info[e.Channel.Id].processCommand(e));
                }
                catch (Exception ex)
                {
                   Log.ErrorAt(info[e.Channel.Id].name + "@Event", ex.Message);
                }
            }
        }

        public EventInformation GetInfoFromSQL(string name, ulong id)
        {
            string myConnection = "datasource=localhost;port=3306;Initial Catalog='ia';username=root;password=laikaxx1";
            MySqlConnection connection = new MySqlConnection(myConnection);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM event_information_" + name +" WHERE id = " + id);
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            r.Read();

            EventInformation eventOutput = new EventInformation();
            eventOutput.name = r.GetString(1);
            eventOutput.enabled = r.GetBoolean(2);
            connection.Close();
            return new EventInformation(null);
        }
    }
}
