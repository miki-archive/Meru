using IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_TestBot
{
    class Program
    {
        static void Main(string[] argv)
        {
            IABot bot = new IABot(botInfo =>
            {
                botInfo.botName = "ia";
                botInfo.botToken = "MTg4Nzg0MDczMjYxNDQ5MjE3.Cokn6Q.p-NzzIypidnm0webbSNiGmgXiO4";
            });

            bot.AddEvent(ev =>
            {
                ev.name = "welcome";
                ev.processCommand = (e, args) =>
                {
                    e.Server.DefaultChannel.SendMessage(e.User.Name + " has joined the server! Hello");
                };
            });

            bot.Connect();
        }
    }
}
