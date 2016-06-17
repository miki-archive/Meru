using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Discord;

namespace IA.Data
{
    public class SQL
    {
        public static string myConnection = "datasource=localhost;port=3306;Initial Catalog='ia';username=root;password=laikaxx1";
        public static MySqlConnection connection = new MySqlConnection(myConnection);

        public static string Query(string SQLCode, Channel c = null, bool sendFeedBack = false)
        {
            try
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = SQLCode;
                connection.Open();
                command.ExecuteNonQuery();
                if (c != null)
                {
                    c.SendMessage(":white_check_mark: Sent `" + command.CommandText + "` to DB!");
                }
                Log.DoneAt("mysql", "Connected!");
                connection.Close();
                return "";
            }
            catch (Exception ex)
            {
                if (c != null)
                {
                    c.SendMessage(":no_entry_sign: " + ex.Message);
                }
                Log.ErrorAt("mysql", ex.Message);
                connection.Close();
                return "";
            }
        }

        public static string GetQuery(string SQLCode)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = SQLCode;
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            r.Read();
            string output = r.GetString(0);
            connection.Close();
            return output;
        }
    }
}
