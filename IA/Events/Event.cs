using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public delegate EventInformation CommandInfoDelegate(EventInformation info);

    class Event
    {
        protected EventInformation info;

        public Event(EventInformation info)
        {
            this.info = info;
            Log.Notice("Loaded Event '" + info.name + "'");
        }

        public void SetActive(bool value)
        {
            info.enabled = value;
        }

        public async void Trigger(MessageEventArgs e)
        {
            if (info.enabled)
            {
                try
                {
                    if (info.developerOnly && e.User.Id != Global.DeveloperId) return;
                    if ((info.adminOnly && !e.User.ServerPermissions.Administrator)) return;
                    await Task.Run(() => info.processCommand(e));
                }
                catch (Exception ex)
                {
                   await e.Channel.SendMessage("[:no_entry_sign: @EventListener.Trigger()] " + ex.Message);
                }
            }
        }

        public string GetName()
        {
            return info.name;
        }
    }
}
