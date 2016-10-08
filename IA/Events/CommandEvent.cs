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

        public ProcessCommand processCommand = async (e, args) =>
        {
            await e.Channel.SendMessageSafeAsync("This command has not been set up properly.");
        };

        public CommandEvent()
        {
            CommandUsed = 0;
        }

        public async Task Check(IMessage e, string identifier = "")
        {
            IGuildChannel guild = (e.Channel as IGuildChannel);

            // declaring variables
            string command = e.Content.Substring(identifier.Length).Split(' ')[0];
            string args = "";
            if (e.Content.Split(' ').Length > 1)
            {
                args = e.Content.Substring(e.Content.Split(' ')[0].Length + 1);
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

            if (IsOnCooldown(e.Author.Id))
            {
                await e.Channel.SendMessageSafeAsync($"Sorry, this command is still on cooldown for {GetCooldown(e.Author.Id)} seconds!");
                return;
            }

            if (checkCommand(e, command, allAliases))
            {
                try
                {
                    if (TryProcessCommand(e, args))
                    {
                        Log.Message(name + " called from " + guild.Name + " [" + guild.Id + " # " + e.Channel.Id + "]");
                        CommandUsed++;
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorAt(name, ex.Message);
                    await e.Channel.SendMessageSafeAsync(errorMessage);
                }
            }
        }

        public bool TryProcessCommand(IMessage e, string args)
        {
            try
            {
                processCommand(e, args);
                return true;
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
            return false;
        }

        float GetCooldown(ulong id)
        {
            float currentCooldown = (float)(DateTime.Now.AddSeconds(-cooldown) - lastTimeUsed[id]).TotalSeconds;
            // do stuff?
            return currentCooldown;
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
