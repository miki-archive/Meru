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

        public async void Check(UserEventArgs e)
        {
            if (enabled[e.Server.Id])
            {

            }
        }
    }
}
