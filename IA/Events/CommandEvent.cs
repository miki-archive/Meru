using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class CommandEvent : Event
    {
        public int cooldown = 1;

        public CheckCommand checkCommand = (e, command, aliases) =>
        {
            return aliases.Contains(command);
        };

        public ProcessCommand processCommand = (e, args) =>
        {
            e.Channel.SendMessage("This command has not been set up properly.");
        };

        public CommandEvent()
        {
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
            string[] allAliases = new string[aliases.Length + 1];
            int i = 0;

            // loading aliases
            foreach (string a in allAliases)
            {
                allAliases[i] = a;
                i++;
            }
            allAliases[allAliases.Length - 1] = name;

            if (enabled.ContainsKey(e.Channel.Id))
            {
                if (!enabled[e.Channel.Id]) return;
            }
            else
            {

            }

            if (!origin.developers.Contains(e.User.Id))
            {
                if (accessibility == EventAccessibility.DEVELOPERONLY)
                {
                    return;
                }

                if (accessibility != EventAccessibility.PUBLIC)
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

            if (checkCommand(e, command, allAliases))
            {
                try
                {
                    processCommand(e, args);
                    Log.Message(name + " called from " + e.Server.Name + " [" + e.Server.Id + " # " + e.Channel.Id + "]");
                    CommandUsed++;
                }
                catch (Exception ex)
                {
                    Log.ErrorAt(name, ex.Message);
                    await e.Channel.SendMessage(errorMessage);
                }
            }
        }
        bool IsOnCooldown(ulong id)
        {
            if (lastTimeUsed.ContainsKey(id))
            {
                if (DateTime.Now.AddSeconds(-cooldown) >= lastTimeUsed[id])
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
