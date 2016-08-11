using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Event
    {
        public EventInformation info;

        public Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();
        protected Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public int CommandUsed { protected set; get; }

        public Event()
        {
            info = new EventInformation();
            CommandUsed = 0;
        }

        public Event(Action<EventInformation> info)
        {
            info.Invoke(this.info);
        }

       

        public static async Task PrintMessage(MessageEventArgs e, string message, DeleteSelf s = null)
        {
            Message m = await e.Channel.SendMessage(message);
            if (s != null)
            {
                await Task.Run(() => DeleteMessage(s, m)); 
            }
        }

        static async Task DeleteMessage(DeleteSelf s, Message message)
        {
            await Task.Delay(s.Seconds * 1000);
            await message.Delete();
        }

        public void SetEnabled(MessageEventArgs e, bool v)
        {
            ulong id = 0;

            switch (info.range)
            {
                case EventRange.CHANNEL: id = e.Channel.Id; break;
                case EventRange.SERVER: id = e.Server.Id; break;
                case EventRange.USER: id = e.User.Id; break;    
            }

            if(!enabled.ContainsKey(e.Channel.Id))
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
