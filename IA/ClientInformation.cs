using IA.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA
{
    public class ClientInformation
    {
        public string botName = "IABot";
        public string botVersion = "1.0.0";

        public string botToken = "";
        public string botIdentifier = ">";

        public LogLevel logLevel = LogLevel.ERROR;

        public SQLInformation sqlInformation;

        public bool CanLog(LogLevel level)
        {
            return logLevel >= level;
        }

        public string GetSQLConnectionString()
        {
            return sqlInformation.GetConnectionString();
        }
        public string GetVersion()
        {
            return "v" + botVersion;
        }
    }

    public enum LogLevel
    {
        ALL,
        INFO,
        ERROR
    }
}
