using Discord;
using IA.Logging;
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
        Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public int CommandUsed { private set; get; }

        public Event()
        {
            info = new EventInformation();
            CommandUsed = 0;
        }

        public Event(Action<EventInformation> info)
        {
            info.Invoke(this.info);
        }

        public async Task Check(MessageEventArgs e, string identifier = "")
        {
            string command = e.Message.RawText.Substring(identifier.Length).Split(' ')[0];
            string args = "";
            if (e.Message.RawText.Split(' ').Length > 1)
            {
                args = e.Message.RawText.Substring(e.Message.RawText.Split(' ')[0].Length + 1);
            }
            string[] aliases = new string[info.aliases.Length + 1];
            int i = 0; 
            foreach (string a in info.aliases)
            {
                aliases[i] = a;
                i++;
            }
            aliases[aliases.Length - 1] = info.name;

            if (!info.origin.developers.Contains(e.User.Id))
            {
                if (info.accessibility == EventAccessibility.DEVELOPERONLY)
                {
                    return;
                }

                if (info.accessibility != EventAccessibility.PUBLIC)
                {
                    if (!e.User.ServerPermissions.Administrator)
                    {
                        return;
                    }
                }

                if (IsOnCooldown(e.User.Id))
                {
                    await e.Channel.SendMessage("Sorry, this command is still on cooldown!");
                    return;
                }
            }

            if(info.checkCommand(command, aliases, e))
            {
                try
                {
                    info.processCommand(e, args);
                }
                catch(Exception ex)
                {
                    Log.ErrorAt(info.name, ex.Message);
                }
                Log.Message(info.name + " called from " + e.Server.Name + " [" + e.Server.Id + " # " + e.Channel.Id + "]");
                CommandUsed++;
            }
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

        bool IsOnCooldown(ulong id)
        {
            if (lastTimeUsed.ContainsKey(id))
            {
                if (DateTime.Now.AddSeconds(-info.cooldown) >= lastTimeUsed[id])
                {
                    lastTimeUsed[id] = DateTime.Now;
                    return false;
                }
                return true;
            }
            else
            {
                lastTimeUsed.Add(id, DateTime.Now);
                return false;
            }
        }

        public void SetEnabled(MessageEventArgs e, bool v)
        {
            if(!enabled.ContainsKey(e.Channel.Id))
            {
                enabled.Add(e.Channel.Id, v);
            }
            else
            {
                enabled[e.Channel.Id] = v;
            }
        }
    }
}
