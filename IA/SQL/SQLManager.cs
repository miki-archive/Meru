using Discord;
using IA.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SQL
{
    public class SQLManager
    {
        SQLInformation info;
        string defaultIdentifier;
        static SQLManager instance;

        public SQLManager()
        {
            if (instance == null)
            {
                Log.ErrorAt("IA.SQLManager", "IA not initialized");
                return;
            }

            info = instance.info;
            defaultIdentifier = instance.defaultIdentifier;
        }

        public SQLManager(SQLInformation info, string defaultIdentifier = ">")
        {
            this.info = info;
            this.defaultIdentifier = defaultIdentifier;
            instance = this;
        }

        public int IsEventEnabled(string name, ulong channelId)
        {
            if (info == null) return 1;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            DbCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT enabled FROM event WHERE id=\"{channelId}\" AND name=\"{name}\"";

            connection.Open();
            DbDataReader r = command.ExecuteReader();

            bool output = false;
            string check = "";

            while (r.Read())
            {
                output = r.GetBoolean(0);
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
            DbCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT i FROM identifier WHERE id={0}", server);
            connection.Open();
            DbDataReader r = command.ExecuteReader();

            while (r.Read())
            {
                output = r.GetString(0);
                break;
            }
            connection.Close();

            if (output != "")
            {
                return output;
            }
            return "ERROR";
        }

        public string GetConnectionString()
        {
            return info.GetConnectionString();
        }

        public static SQLManager GetInitializedObject()
        {
            return instance;
        }

        public void SendToSQL(string sqlCode)
        {
            if (info == null) return;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            DbCommand command = connection.CreateCommand();
            command.CommandText = sqlCode;
            connection.Open();
            DbDataReader r = command.ExecuteReader();
            connection.Close();
        }

        public void TryCreateTable(string sqlCode)
        {
            if (info == null) return;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            DbCommand command = connection.CreateCommand();
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {sqlCode}";
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
}

