using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate bool CheckCommand(string command, string[] allAliases, MessageEventArgs e);
    public delegate void ProcessCommand(MessageEventArgs e, string args);

    public enum EventAccessibility
    {
        PUBLIC,
        ADMINONLY,
        DEVELOPERONLY
    }

    public enum EventType
    {
        COMMAND,
        MENTION,
        ONJOIN,
        ONLEAVE
    }

    public class EventInformation
    {
        public string name = "name not set";
        public string[] aliases = new string[0];

        public string description;
        public string[] usage = new string[0];

        public bool enabled = true;
        public bool deletesMessage = false;
        public DeleteSelf deletesSelf = null;

        public int cooldown = 1;

        public Module parent;

        public EventAccessibility accessibility = EventAccessibility.PUBLIC;
        public EventType type = EventType.COMMAND;

        public CheckCommand checkCommand = (command, aliases, e) =>
        {
            return aliases.Contains(command);
        };
        public ProcessCommand processCommand = (e, args) =>
        {
            e.Channel.SendMessage("This command has not been set up properly.");
        };

        public EventInformation()
        {

        }
        public EventInformation(Action<EventInformation> info)
        {
            info.Invoke(this);
        }
    }

    public class DeleteSelf
    {
        public int Seconds;

        public DeleteSelf(int seconds)
        {
            Seconds = seconds;
        }
    }
}
