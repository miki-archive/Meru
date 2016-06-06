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
        public static async Task<string> Query(string SQLCode, Channel c, bool sendFeedBack = false)
        {
            try
            {
                string myConnection = "datasource=localhost;port=3306;Initial Catalog='ia';username=root;password=laikaxx1";
                MySqlConnection connection = new MySqlConnection(myConnection);
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = SQLCode;
                connection.Open();
                command.ExecuteNonQuery();
                if (c != null)
                {
                    await c.SendMessage(":white_check_mark: Sent `" + command.CommandText + "` to DB!");
                }
                Log.DoneAt("mysql", "Connected!");
                connection.Close();
                return "";
            }
            catch (Exception ex)
            {
                if (c != null)
                {
                    await c.SendMessage(":no_entry_sign: " + ex.Message);
                }
                Log.ErrorAt("mysql",ex.Message);
                return "";
            }
        }
    }
}
