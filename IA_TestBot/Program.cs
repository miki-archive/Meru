using Discord;
using DynamicExpresso;
using IA;
using IA.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_TestBot
{
    class Program
    {
        static ulong setChannel = 0;

        static void Main(string[] argv)
        {
            Bot bot = new Bot();

          
            bot.Connect();
            Console.ReadLine();
        }
    }
}