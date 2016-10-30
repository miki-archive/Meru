using Discord;
using IA.SDK;
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

        public GuildPermission[] requiresPermissions = new GuildPermission[0];

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
        public CommandEvent(Action<CommandEvent> info)
        {
            info.Invoke(this);
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
                await e.Channel.SendMessageSafeAsync($"Sorry, this command is still on cooldown for {-GetCooldown(e.Author.Id)} seconds!");
                return;
            }

            if (requiresPermissions.Length > 0)
            {
                foreach (GuildPermission g in requiresPermissions)
                {
                    if (!await hasPermission(e.Channel as IGuildChannel, g))
                    {
                        await e.Channel.SendMessageSafeAsync($"Please give me `{g}` to use this command.");
                        return;
                    }
                }
            }

            if (checkCommand(e, command, allAliases))
            {
                if (await TryProcessCommand(e, args))
                {
                    Log.Message(name + " called from " + guild.Name + " [" + guild.Id + " # " + e.Channel.Id + "]");
                    await eventSystem.OnCommandDone(e, this);
                    CommandUsed++;
                }
            }
        }

        public async Task<bool> TryProcessCommand(IMessage e, string args)
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
            float currentCooldown = (float)(DateTime.Now.AddSeconds(-cooldown) - lastTimeUsed[id]).TotalSeconds;
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
    
        async Task<bool> hasPermission(IGuildChannel e, GuildPermission r)
        {
            return (await e.GetUserAsync(Bot.instance.Client.CurrentUser.Id)).GuildPermissions.Has(r);
        }
    }
}
