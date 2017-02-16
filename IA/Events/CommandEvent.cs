using IA.SDK;
using IA.SDK.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeCommandEvent : Event
    {
        public int cooldown = 1;

        public DiscordGuildPermission[] requiresPermissions = new SDK.DiscordGuildPermission[0];

        public CheckCommand checkCommand = (e, command, aliases) =>
        {
            return aliases.Contains(command);
        };

        public ProcessCommand processCommand = async (e, args) =>
        {
            await e.Channel.SendMessage("This command has not been set up properly.");
        };

        public RuntimeCommandEvent()
        {
            CommandUsed = 0;
        }

        public RuntimeCommandEvent(Action<RuntimeCommandEvent> info)
        {
            info.Invoke(this);
            CommandUsed = 0;
        }

        public async Task Check(IDiscordMessage e, string identifier = "")
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
                if (!enabled[e.Channel.Id])
                {
                    return;
                }
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
                        await e.Channel.SendMessage($"Please give me the server permission `{g}` to use this command.");
                        return;
                    }
                }
            }

            if (checkCommand(e, command, allAliases))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (await TryProcessCommand(e, args))
                {
                    await eventSystem.OnCommandDone(e, this);
                    CommandUsed++;
                    Log.Message($"{name} called from {e.Guild.Name} in {sw.ElapsedMilliseconds}ms");
                }
                sw.Stop();
            }
        }

        private float GetCooldown(ulong id)
        {
            float currentCooldown = (float)(DateTime.Now.AddSeconds(-cooldown) - lastTimeUsed[id]).TotalSeconds;
            return currentCooldown;
        }

        private bool IsOnCooldown(ulong id)
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

        private async Task<bool> TryProcessCommand(IDiscordMessage e, string args)
        {
            try
            {
                await processCommand(e, args);
                return true;
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessage(errorMessage);
                Log.ErrorAt(name, ex.Message + "\n" + ex.StackTrace);
            }
            return false;
        }
    }
}