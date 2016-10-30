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
        public string Name { get; set; } = "IABot";
        public string Version { get; set; } = "1.0.0";

        public string Token { get; set; } = "";
        public string Prefix { get; set; } = ">";

        public string CarbonitexKey { get; set; } = "";
        public string DiscordPwKey { get; set; } = "";

        public int ShardCount { get; set; } = 1;
        public int ShardId { get; internal set; } = -1;

        public LogLevel ConsoleLogLevel = LogLevel.ALL;
        /// <summary>
        /// Saves logs to ./logs/xxxxx.log
        /// </summary>
        public LogLevel FileLogLevel = LogLevel.ERROR;

        public SQLInformation sqlInformation;

        public bool CanLog(LogLevel level)
        {
            return ConsoleLogLevel <= level;
        }
        public bool CanFileLog(LogLevel level)
        {
            return FileLogLevel <= level;
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
