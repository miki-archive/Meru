using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate Task UseCommand(MessageEventArgs e);

    public class EventInformation
    {
        public string name;
        public string description;
        public string moduleName;

        public bool deleteTrigger;

        public bool adminOnly;
        public bool developerOnly;
        public bool enabled;

        public UseCommand processCommand;

        public EventInformation()
        {

        }

        public EventInformation(EventInformation info)
        {
            
        }

        public EventInformation(string name, UseCommand command)
        {
            this.name = name;
            enabled = true;
            adminOnly = false;
            developerOnly = false;

            processCommand = command;
        }
    }
}
