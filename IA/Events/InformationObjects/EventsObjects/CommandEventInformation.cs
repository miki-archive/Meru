using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events.InformationObjects
{
    public delegate void ProcessCommand(MessageEventArgs e, string args);
    public delegate bool CheckCommand(MessageEventArgs e, string command, string[] allAliases);


    public class CommandEventInformation : EventInformation
    {
        public int cooldown = 1;

        public CheckCommand checkCommand = (e, command, aliases) =>
        {
            return aliases.Contains(command);
        };

        public ProcessCommand processCommand = (e, args) =>
        {
            e.Channel.SendMessage("This command has not been set up properly.");
        };
    }
}
