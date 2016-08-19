using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public enum EventAccessibility
    {
        PUBLIC,
        ADMINONLY,
        DEVELOPERONLY
    }

    public enum EventRange
    {
        USER,
        CHANNEL,
        SERVER
    }

    public delegate void ProcessServerCommand(UserEventArgs e);
    public delegate void ProcessCommand(MessageEventArgs e, string args);
    public delegate bool CheckCommand(MessageEventArgs e, string command, string[] allAliases);
}
