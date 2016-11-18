using Discord;
using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeCommandEvent : RuntimeEvent, IToggleable, IProcessable
    {
        public RuntimeCommandEvent()
        {
        }
        public RuntimeCommandEvent(Action<RuntimeCommandEvent> info)
        {
            info.Invoke(this);
        }
        public RuntimeCommandEvent(SdkCommandEvent e)
        {
            Name = e.Name;

            processCommand = e.processCommand;
            checkCommand = e.checkCommand;

            accessibility = e.accessibility;
            aliases = e.Aliases;
            canBeDisabled = e.CanBeDisabled;
            cooldown = e.SecondsCooldown;
            defaultEnabled = e.DefaultEnabled;
            description = e.Description;
            errorMessage = e.ErrorMessage;
            module = e.module;
            requiresPermissions = e.requiresPermissions;
            usage = e.Usage;
        }

        public async Task Check(RuntimeMessage e, string identifier = "")
        {
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
                await e.Channel.SendMessage($"Sorry, this command is still on cooldown for {-GetCooldown(e.Author.Id)} seconds!");
                return;
            }

            if (requiresPermissions.Length > 0)
            {
                foreach (DiscordGuildPermission g in requiresPermissions)
                {
                    if (!e.Author.HasPermissions(e.Channel, g))
                    {
                        await e.Channel.SendMessage($"Please give me `{g}` to use this command.");
                        return;
                    }
                }
            }

            if (checkCommand(e, command, allAliases))
            {
                if (await TryProcessCommand(e, args))
                {
                    Log.Message($"{name} called from {/* name here */ name } [{ e.Guild.Id } # { e.Channel.Id }]");

                    //   await eventSystem.OnCommandDone(e, this);
                    TimesProcessed++;
                }
            }
        }



        public async Task<bool> TryProcessCommand(RuntimeMessage e, string args)
        {
            try
            {
                await processCommand(e, args);
            }
            catch(Exception ex)
            {
                Log.ErrorAt(name, ex.Message);
            }
            return true;
        }

        float GetCooldown(ulong id)
        {
            float currentCooldown = (float)(DateTime.Now.AddSeconds(-cooldown) - LastUsed[id]).TotalSeconds;
            return currentCooldown;
        }

        bool IsOnCooldown(ulong id)
        {
            if (LastUsed.ContainsKey(id))
            {
                if (DateTime.Now.AddSeconds(-cooldown) >= LastUsed[id])
                {
                    LastUsed[id] = DateTime.Now;
                    return false;
                }
                return true;
            }
            else
            {
                LastUsed.Add(id, DateTime.Now);
                return false;
            }
        }
    }
}
