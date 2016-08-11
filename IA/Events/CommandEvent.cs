using Discord;
using IA.Events.InformationObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class CommandEvent : Event
    {
        public new CommandEventInformation info;

        public CommandEvent()
        {
            info = new CommandEventInformation();
            CommandUsed = 0;
        }

        public async Task Check(MessageEventArgs e, string identifier = "")
        {
            // declaring variables
            string command = e.Message.RawText.Substring(identifier.Length).Split(' ')[0];
            string args = "";
            if (e.Message.RawText.Split(' ').Length > 1)
            {
                args = e.Message.RawText.Substring(e.Message.RawText.Split(' ')[0].Length + 1);
            }
            string[] aliases = new string[info.aliases.Length + 1];
            int i = 0;

            // loading aliases
            foreach (string a in info.aliases)
            {
                aliases[i] = a;
                i++;
            }
            aliases[aliases.Length - 1] = info.name;

            if (enabled.ContainsKey(e.Channel.Id))
            {
                if (!enabled[e.Channel.Id]) return;
            }
            else
            {

            }

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

            if (info.checkCommand(e, command, aliases))
            {
                try
                {
                    info.processCommand(e, args);
                    Log.Message(info.name + " called from " + e.Server.Name + " [" + e.Server.Id + " # " + e.Channel.Id + "]");
                    CommandUsed++;
                }
                catch (Exception ex)
                {
                    Log.ErrorAt(info.name, ex.Message);
                    await e.Channel.SendMessage(info.errorMessage);
                }
            }
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
    }
}
