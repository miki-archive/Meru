using IA.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.UserData
{
    class Achievements
    {

        Dictionary<string, AchievementData> achievements = new Dictionary<string, AchievementData>();

        public Achievements()
        {

        }

        public Achievement GetFromSQL(ulong id)
        {
            Achievement a = new Achievement(x => {
            });
            using (SqlConnection con = new SqlConnection())
            {
                con.Open();
                using (SqlCommand command = new SqlCommand("SELECT FROM achievements WHERE id LIKE " + id, con))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string _name = reader.GetString(1);
                        bool _unlocked = reader.GetBoolean(2);

                        a = new Achievement(x =>
                        {
                            x.name = _name;
                            x.unlocked = _unlocked;
                        });
                    }
                }
            }
            return a;
        }

        public void SendToSQL()
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.Open();
                try
                {
                    foreach (KeyValuePair<string, AchievementData> item in achievements)
                    {
                        using (SqlCommand command = new SqlCommand(
                            "INSERT INTO achievements VALUES(@name, @unlocked)", con))
                        {
                            command.Parameters.Add(new SqlParameter("name", item.Key));
                            command.Parameters.Add(new SqlParameter("unlocked", item.Value.unlocked));
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Count not insert.");
                }
            }
        }
    }
}
