using Discord;
using IA.Events.InformationObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class UserEvent:Event
    {
        public new UserEventInformation info;

        public UserEvent()
        {
            info = new UserEventInformation();
            CommandUsed = 0;
        }

        public async Task Check(UserEventArgs e)
        {
            await Task.Run(() => info.processCommand(e));
        }
    }
}
