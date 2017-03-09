using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeCommandEvent : Event, ICommandEvent
    {
        public int Cooldown { get; set; }

        public List<DiscordGuildPermission> GuildPermissions { get; set; } = new List<DiscordGuildPermission>();

        public CheckCommandDelegate CheckCommand { get; set; } = (e, args, aliases) =>
        {
            return true;
        };

        public ProcessCommandDelegate ProcessCommand { get; set; } = async (e, args) =>
        {

        };

        public RuntimeCommandEvent() { }
        public RuntimeCommandEvent(ICommandEvent commandEvent) : base(commandEvent)
        {
            CheckCommand = commandEvent?.CheckCommand;
            Cooldown = commandEvent.Cooldown;
            GuildPermissions = commandEvent?.GuildPermissions;
            ProcessCommand = commandEvent?.ProcessCommand;
        }
        public RuntimeCommandEvent(Action<RuntimeCommandEvent> info) { info.Invoke(this);  }

        public async Task Check(IDiscordMessage e, string identifier = "")
        {
            // declaring variables
            string command = e.Content.Substring(identifier.Length).Split(' ')[0];
            string args = "";
            if (e.Content.Split(' ').Length > 1)
            {
                args = e.Content.Substring(e.Content.Split(' ')[0].Length + 1);
            }

            string[] allAliases = null;

            if (Aliases != null)
            {
                allAliases = new string[Aliases.Length + 1];
                int i = 0;

                // loading aliases
                foreach (string a in allAliases)
                {
                    allAliases[i] = a;
                    i++;
                }

                allAliases[allAliases.Length - 1] = Name;
            }

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

            if (GuildPermissions.Count > 0)
            {
                foreach (DiscordGuildPermission g in GuildPermissions)
                {
                    if (!e.Author.HasPermissions(e.Channel, g))
                    {
                        await e.Channel.SendMessage($"Please give me the guild permission `{g}` to use this command.");
                        return;
                    }
                }
            }

            if (CheckCommand(e, command, allAliases))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (await TryProcessCommand(e, args))
                {
                    await eventSystem.OnCommandDone(e, this);
                    TimesUsed++;
                    Log.Message($"{Name} called from {e.Guild.Name} in {sw.ElapsedMilliseconds}ms");
                }
                sw.Stop();
            }
        }

        private float GetCooldown(ulong id)
        {
            float currentCooldown = (float)(DateTime.Now.AddSeconds(-Cooldown) - lastTimeUsed[id]).TotalSeconds;
            return currentCooldown;
        }

        private bool IsOnCooldown(ulong id)
        {
            if (lastTimeUsed.ContainsKey(id))
            {
                if (DateTime.Now.AddSeconds(-Cooldown) >= lastTimeUsed[id])
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
                await ProcessCommand(e, args);
                return true;
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessage(Metadata.errorMessage);
                Log.ErrorAt(Name, ex.Message + "\n" + ex.StackTrace);
            }
            return false;
        }
    }
}