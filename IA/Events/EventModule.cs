using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class EventModule
    {
        public EventModuleInformation moduleInformation { get; private set; }

        public EventModule(Action<EventModuleInformation> moduleInfo)
        {
            moduleInformation = new EventModuleInformation();
            moduleInfo.Invoke(moduleInformation);
        }

        public void SetActive(bool value)
        {
            moduleInformation.enabled = value;
        }
    }
}
