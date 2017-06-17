using System.Threading.Tasks;

namespace Meru
{
    public delegate Task LoadEvents(Client bot);

    public class ClientInformation
    {
        public string Name { get; set; } = "IABot";
        public string Version { get; set; } = "1.0.0";

        public string Token { get; set; } = "";

        public int ShardCount { get; set; } = 1;

        internal int ShardId { get; set; } = -1;

        public LoadEvents EventLoaderMethod { get; set; }

        public LogLevel ConsoleLogLevel = LogLevel.NOTICE;

        /// <summary>
        /// Saves logs to ./logs/xxxxx.log
        /// </summary>
        public LogLevel FileLogLevel = LogLevel.ERROR;

        public bool CanLog(LogLevel level)
        {
            return ConsoleLogLevel <= level;
        }

        public bool CanFileLog(LogLevel level)
        {
            return FileLogLevel <= level;
        }
    }

    public enum LogLevel
    {
        ALL,
        VERBOSE,
        NOTICE,
        MESSAGE,
        WARNING,
        ERROR,
        NONE
    }
}