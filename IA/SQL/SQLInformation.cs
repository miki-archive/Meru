using System;

namespace IA.Sql
{
    public class SQLInformation
    {
        public string dataSource = "";
        public int port = 3306;

        public string database = "";

        public string username = "root";
        public string password = "";

        public string GetConnectionString()
        {
            return string.Format("datasource={0};port={1};Initial Catalog='{2}';username={3};password={4}", dataSource, port, database, username, password);
        }

        public SQLInformation(Action<SQLInformation> info)
        {
            info.Invoke(this);
        }
    }
}