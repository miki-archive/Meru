using Discord;
using IA.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SQL
{
    public class SQLManager
    {
        SQLInformation info;
        string defaultIdentifier;

        public SQLManager(SQLInformation info, string defaultIdentifier = ">")
        {
            this.info = info;
            this.defaultIdentifier = defaultIdentifier;
        }

        public int IsEventEnabled(string name, ulong channelId)
        {
            if (info == null) return 1;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT enabled FROM event WHERE id={1} AND name='{0}'", name, channelId);
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();

            bool output = false;
            string check = "";

            while (r.Read())
            {
                output = r.GetBoolean("enabled");
                check = "ok";
                break;
            }
            connection.Close();

            if (check == "") return -1;
            return output?1:0;
        }

        public async void SetIdentifier(string identifier, ulong server)
        {
            await Task.Run(() => SendToSQL("INSERT INTO identifier VALUES(" + server + ", \"" + identifier + "\")"));
        }
        public string GetIdentifier(ulong server)
        {
            if (info == null) return defaultIdentifier;

            string output = "";

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT i FROM identifier WHERE id={0}", server);
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();

            while (r.Read())
            {
                output = r.GetString("i");
                break;
            }
            connection.Close();

            if (output != "")
            {
                return output;
            }
            return "ERROR";
        }

        public void SendToSQL(string sqlcode)
        {
            if (info == null) return;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sqlcode;
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            connection.Close();
        }
    }
}