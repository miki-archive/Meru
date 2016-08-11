using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events.InformationObjects
{
    public delegate void ProcessServerCommand(UserEventArgs e);

    public class UserEventInformation : EventInformation
    {
        public ProcessServerCommand processCommand = (e) =>
        {
            e.Server.DefaultChannel.SendMessage("This serverevent has not been set up correctly.");
        };
    }
}
