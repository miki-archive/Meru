using IA.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA
{
    public  class ClientInformation
    {
        public string Name = "IABot";
        public string Version = "1.0.0";

        public string Token = "";
        public string Prefix = ">";

        public int shardCount = 1;

        public LogLevel logLevel = LogLevel.ALL;
        /// <summary>
        /// Saves logs to ./logs/xxxxx.log
        /// </summary>
        public LogLevel fileLogLevel = LogLevel.ERROR;

        public SQLInformation sqlInformation;

        public bool CanLog(LogLevel level)
        {
            return logLevel <= level;
        }
        public bool CanFileLog(LogLevel level)
        {
            return fileLogLevel <= level;
        }

        public string GetSQLConnectionString()
        {
            return sqlInformation.GetConnectionString();
        }
        public string GetVersion()
        {
            return "v" + Version;
        }
    }

    public enum LogLevel
    {
        ALL,
        NOTICE,
        MESSAGE,
        WARNING,
        ERROR,
        NONE
    }
}
