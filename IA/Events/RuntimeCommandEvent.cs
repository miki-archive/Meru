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
        public Dictionary<string, ProcessCommandDelegate> CommandPool { get; set; } = new Dictionary<string, ProcessCommandDelegate>();
        public int Cooldown { get; set; }

        public List<DiscordGuildPermission> GuildPermissions { get; set; } = new List<DiscordGuildPermission>();

        public CheckCommandDelegate CheckCommand { get; set; } = (e, args, aliases) => true;
        public ProcessCommandDelegate ProcessCommand { get; set; } = async (e, args) => { };

        public RuntimeCommandEvent() { }
        public RuntimeCommandEvent(string name) { Name = name; }
        public RuntimeCommandEvent(ICommandEvent commandEvent) : base(commandEvent)
        {
            CheckCommand = commandEvent?.CheckCommand;
            Cooldown = commandEvent.Cooldown;
            GuildPermissions = commandEvent?.GuildPermissions;
            ProcessCommand = commandEvent?.ProcessCommand;
            CommandPool = commandEvent?.CommandPool;
        }
        public RuntimeCommandEvent(Action<RuntimeCommandEvent> info) { info.Invoke(this);  }

        public async Task Check(IDiscordMessage e, string identifier = "")
        {
            string command = e.Content.Substring(identifier.Length).Split(' ')[0];
            string args = "";
            List<string> allAliases = new List<string>();
            string[] arguments = new string[0];

            if (e.Content.Split(' ').Length > 1)
            {
                args = e.Content.Substring(e.Content.Split(' ')[0].Length + 1);
                arguments = args.Split(' ');
            }

            if(Module != null)
            {
                if(Module.Nsfw && !e.Channel.Nsfw)
                {
                    return;
                }
            }

            if (Aliases != null)
            {
                allAliases.AddRange(Aliases);
                allAliases.Add(Name);
            }

            if (enabled.ContainsKey(e.Channel.Id))
            {
                if (!enabled[e.Channel.Id])
                {
                    Log.WarningAt(Name, " is disabled");
                    return;
                }
            }

            if (IsOnCooldown(e.Author.Id))
            {
                await e.Channel.SendMessage($"Sorry, this command is still on cooldown for {-GetCooldown(e.Author.Id)} seconds!");
                Log.WarningAt(Name, " is on cooldown");
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

            if (CheckCommand(e, command, allAliases.ToArray()))
            {
                ProcessCommandDelegate targetCommand = ProcessCommand;

                if(arguments.Length > 0)
                {
                    if(CommandPool.ContainsKey(arguments[0]))
                    {
                        targetCommand = CommandPool[arguments[0]];
                        args = args.Substring((arguments[0].Length == args.Length) ? arguments[0].Length : arguments[0].Length + 1);
                    }
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (await TryProcessCommand(targetCommand, e, args))
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

        private async Task<bool> TryProcessCommand(ProcessCommandDelegate cmd, IDiscordMessage e, string args)
        {
            try
            {
                await cmd(e, args);
                return true;
            }
            catch(Exception ex)
            {
                Log.ErrorAt(Name, ex.Message + "\n" + ex.StackTrace);
            }
            return false;
        }

        public ICommandEvent SetCooldown(int seconds)
        {
            Cooldown = seconds;
            return this;
        }

        public ICommandEvent SetPermissions(params DiscordGuildPermission[] permissions)
        {
            GuildPermissions.AddRange(permissions);
            return this;
        }

        public ICommandEvent On(string args, ProcessCommandDelegate command)
        {
            CommandPool.Add(args, command);
            return this;
        }

        public ICommandEvent Default(ProcessCommandDelegate command)
        {
            ProcessCommand = command;
            return this;
        }

        new public ICommandEvent SetName(string name)
        {
            Name = name;
            return this;
        }

        new public ICommandEvent SetAccessibility(EventAccessibility accessibility)
        {
            Accessibility = accessibility;
            return this;
        }

        new public ICommandEvent SetAliases(params string[] aliases)
        {
            Aliases = aliases;
            return this;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}