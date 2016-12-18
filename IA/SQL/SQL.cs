using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SQL
{
    public delegate void QueryOutput(Dictionary<string, object> result);

    public class MySQL
    {   
        static MySQL instance;

        SQLInformation info;

        PrefixValue defaultIdentifier;

        public MySQL()
        {
            if (instance == null)
            {
                Log.ErrorAt("IA.SQLManager", "IA not initialized");
                return;
            }

            info = instance.info;
            defaultIdentifier = instance.defaultIdentifier;
        }
        public MySQL(SQLInformation info, PrefixValue defaultIdentifier)
        {
            this.info = info;
            if(defaultIdentifier == null)
            {
                this.defaultIdentifier = new PrefixValue(">");
            }
            else
            {
                this.defaultIdentifier = defaultIdentifier;
            }
            instance = this;
        }

        public string GetConnectionString()
        {
            return info.GetConnectionString();
        }

        /// <summary>
        /// Gets the prefix from the server's id
        /// </summary>
        /// <param name="server_id">server id</param>
        /// <returns></returns>
        public static async Task<string> GetIdentifier(ulong server_id)
        {
            string output = instance.defaultIdentifier.Value;

            await QueryAsync("SELECT i FROM identifier WHERE id=?id", x =>
            {
                if (x != null)
                {
                    output = (string)x["i"];
                }
            }, server_id);

            return output;
        }

        /// <summary>
        /// Gets the instance of the initialized object if created.
        /// </summary>
        /// <returns></returns>
        public static MySQL GetInstance()
        {
            return instance;
        }

        /// <summary>
        /// Queries the sqlCode to output
        /// </summary>
        /// <param name="sqlCode">use this format: UPDATE table.row SET var=?var WHERE var2=?var2</param>
        /// <param name="output"></param>
        public static void Query(string sqlCode, QueryOutput output, params object[] p)
        {
            if (instance.info == null) return;
            MySqlConnection connection = new MySqlConnection(instance.info.GetConnectionString());


            List<MySqlParameter> parameters = new List<MySqlParameter>();

            string curCode = sqlCode;
            string prevCode = "";

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
                    if (splitSql[i].Contains("?"))
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

        /// <summary>
        /// Asynchronously queries the sqlCode to output
        /// </summary>
        /// <param name="sqlCode">use this format: UPDATE table.row SET var=?var WHERE var2=?var2</param>
        /// <param name="output"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static async Task QueryAsync(string sqlCode, QueryOutput output, params object[] p)
        {
            if (instance.info == null) return;
            MySqlConnection connection = new MySqlConnection(instance.info.GetConnectionString());

            List<MySqlParameter> parameters = new List<MySqlParameter>();

            string curCode = sqlCode;
            string prevCode = "";

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
                if(splitString[0].EndsWith(":"))
                {
                    continue;
                }

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
                    if (splitSql[i].Contains("?"))
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

            await Task.Run(async () => await PollQuery(connection, curCode, parameters, output));
        }

        internal static async Task PollQuery(MySqlConnection connection, string CommandText, List<MySqlParameter> parameters, QueryOutput output)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = CommandText;
            command.Parameters.AddRange(parameters.ToArray());
            connection.Open();

            bool hasRead = false;

            if (output != null)
            {
                MySqlDataReader r = await command.ExecuteReaderAsync() as MySqlDataReader;
                Dictionary<string, object> outputdict = new Dictionary<string, object>();

                while (await r.ReadAsync())
                {
                    outputdict = new Dictionary<string, object>();
                    for (int i = 0; i < r.VisibleFieldCount; i++)
                    {
                        outputdict.Add(r.GetName(i), r.GetValue(i));
                    }
                    output?.Invoke(outputdict);
                    hasRead = true;
                }

                if (!hasRead)
                {
                    output?.Invoke(outputdict);
                }
            }
            else
            {
                await command.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        }

        [Obsolete("use 'MySQL.Query' instead.")]
        /// <summary>
        /// Old Query, doesnt return anything.
        /// </summary>
        /// <param name="sqlCode">valid sql code</param>
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

        /// <summary>
        /// Ignores this code if table exists.
        /// </summary>
        /// <param name="sqlCode">valid sql code</param>
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
    }
}

