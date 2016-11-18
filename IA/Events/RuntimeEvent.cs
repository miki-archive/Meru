using Discord;
using IA.SDK;
using IA.SDK.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeEvent : SdkEvent, IEvent, IToggleable
    {
        internal EventSystem eventSystem;

        public int CommandUsed { protected set; get; }

        public RuntimeEvent()
        {
            CommandUsed = 0;
        }
        public RuntimeEvent(Action<RuntimeEvent> info)
        {
            info.Invoke(this);
            CommandUsed = 0;
        }

        public void SetEnabled(ulong channelId, bool v)
        {
            if (!canBeDisabled && !v) return;

            ulong id = 0;

            if(!enabled.ContainsKey(channelId))
            {
                enabled.Add(id, v);
            }
            else
            {
                enabled[id] = v;
            }
        }
    }
}
