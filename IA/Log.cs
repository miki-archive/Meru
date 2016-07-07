using System;

namespace IA.Logging
{
    class Log
    {
        /// <summary>
        /// Display a [msg] message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Message(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[msg]: " + message);
        }

        /// <summary>
        /// Display a [msg] message in the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="arg0"></param>
        public static void Message(string message, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("[msg]: " + message, arg0);
        }

        /// <summary>
        /// Display a [!!!] message.
        /// </summary>
        /// <param name="message"></param>
        public static void Notice(string message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[!!!]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Display a error message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[err]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Display a error message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void ErrorAt(string target, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[err@{0}]: {1}", target, message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Display a warning message.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[wrn]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Display a message when something is done.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void Done(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[yay]: " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Display a message when something is done.
        /// </summary>
        /// <param name="message">information about the action</param>
        public static void DoneAt(string target, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[yay@{0}]: {1}", target, message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
