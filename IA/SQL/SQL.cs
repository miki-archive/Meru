using Discord;
using IA.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Sql
{
    public class SQL
    {
        public delegate void QueryOutput(Dictionary<string, object> result);

        SQLInformation info;
        string defaultIdentifier;
        static SQL instance;

        public SQL()
        {
            if (instance == null)
            {
                Log.ErrorAt("IA.SQLManager", "IA not initialized");
                return;
            }

            info = instance.info;
            defaultIdentifier = instance.defaultIdentifier;
        }

        public SQL(SQLInformation info, string defaultIdentifier = ">")
        {
            this.info = info;
            this.defaultIdentifier = defaultIdentifier;
            instance = this;
        }

        public int IsEventEnabled(string name, ulong channelId)
        {
            if (info == null) return 1;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT enabled FROM event WHERE id=\"{channelId}\" AND name=\"{name}\"";

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

        public static SQL GetInitializedObject()
        {
            return instance;
        }

        public void SendToSQL(string sqlCode)
        {
            if (info == null) return;

            MySqlConnection connection = new MySqlConnection(info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = sqlCode;
            connection.Open();
            MySqlDataReader r = command.ExecuteReader();
            connection.Close();
        }

        public static void TryCreateTable(string sqlCode)
        {
            if (instance.info == null) return;

            MySqlConnection connection = new MySqlConnection(instance.info.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"CREATE TABLE IF NOT EXISTS {sqlCode}";
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Queries the sqlCode to output
        /// </summary>
        /// <param name="sqlCode"></param>
        /// <param name="output"></param>
        public static void Query(string sqlCode, QueryOutput output, params object[] p)
        {
            if (instance.info == null) return;
            MySqlConnection connection = new MySqlConnection(instance.info.GetConnectionString());


            List<MySqlParameter> parameters = new List<MySqlParameter>();

            string curCode = sqlCode;
            string prevCode = "";

            try
            {
                // Get code ready to extract
                while (curCode != prevCode)
                {
                    prevCode = curCode;

                    curCode = curCode.Replace(" = ", "=");
                    curCode = curCode.Replace(" =", "=");
                    curCode = curCode.Replace("= ", "=");
                }

                List<string> splitSql = new List<string>();
                splitSql.AddRange(curCode.Split(' '));

                for (int i = 0; i < splitSql.Count; i++)
                {
                    List<string> splitString = new List<string>();
                    splitString.AddRange(splitSql[i].Split('='));

                    if (splitString.Count > 1)
                    {
                        if (splitString[1].StartsWith("?"))
                        {
                            if (parameters.Find(x => { return x.ParameterName == splitString[0]; }) == null)
                            {
                                parameters.Add(new MySqlParameter(splitString[0], p[parameters.Count]));
                            }
                        }
                    }
                    else
                    {
                        if(splitSql[i].Contains("?"))
                        {
                            splitString = new List<string>();
                            splitString.AddRange(splitSql[i].Split('?'));
                            if (parameters.Find(x => { return x.ParameterName == splitString[1].TrimEnd(',', ')', ';'); }) == null)
                            {
                                parameters.Add(new MySqlParameter(splitString[1].TrimEnd(',', ')', ';'), p[parameters.Count]));
                            }
                        }
                    }
                }

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = curCode;
                command.Parameters.AddRange(parameters.ToArray());
                connection.Open();

                bool hasRead = false;

                if (output != null)
                {
                    MySqlDataReader r = command.ExecuteReader();
                    while (r.Read())
                    {
                        Dictionary<string, object> outputdict = new Dictionary<string, object>();
                        for (int i = 0; i < r.VisibleFieldCount; i++)
                        {
                            outputdict.Add(r.GetName(i), r.GetValue(i));
                        }
                        output?.Invoke(outputdict);
                        hasRead = true;
                    }

                    if (!hasRead)
                    {
                        output?.Invoke(null);
                    }
                }
                else
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception e)
            {
                Log.ErrorAt("mysql.query", e.Message);
            }
            finally
            {
                connection.Close();
            }

        }
        public static async Task QueryAsync(string sqlCode, QueryOutput output, params object[] p)
        {
            if (instance.info == null) return;
            MySqlConnection connection = new MySqlConnection(instance.info.GetConnectionString());


            List<MySqlParameter> parameters = new List<MySqlParameter>();

            string curCode = sqlCode;
            string prevCode = "";

            try
            {
                // Get code ready to extract
                while (curCode != prevCode)
                {
                    prevCode = curCode;

                    curCode = curCode.Replace(" = ", "=");
                    curCode = curCode.Replace(" =", "=");
                    curCode = curCode.Replace("= ", "=");
                }

                List<string> splitSql = new List<string>();
                splitSql.AddRange(curCode.Split(' '));

                for (int i = 0; i < splitSql.Count; i++)
                {
                    List<string> splitString = new List<string>();
                    splitString.AddRange(splitSql[i].Split('='));

                    if (splitString.Count > 1)
                    {
                        parameters.Add(new MySqlParameter(splitString[0], p[parameters.Count]));
                    }
                }

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = curCode;
                command.Parameters.AddRange(parameters.ToArray());
                connection.Open();

                bool hasRead = false;

                MySqlDataReader r = await command.ExecuteReaderAsync() as MySqlDataReader;
                while (await r.ReadAsync())
                {
                    Dictionary<string, object> outputdict = new Dictionary<string, object>();
                    for (int i = 0; i < r.VisibleFieldCount; i++)
                    {
                        outputdict.Add(r.GetName(i), r.GetValue(i));
                    }
                    output(outputdict);
                    hasRead = true;
                }

                if(!hasRead)
                {
                    output(null);
                }

                await connection.CloseAsync();
            }
            catch (Exception e)
            {
                Log.ErrorAt("mysql.query", e.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }

        }


    }
}

