using IA.Models;
using IA.Models.Context;
using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeModule : IModule
    {
        public string Name { get; set; } = "";
        public bool Nsfw { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public bool CanBeDisabled { get; set; } = true;

        public Mutex threadLock;

        public MessageRecievedEventDelegate MessageRecieved { get; set; } = null;
        public UserUpdatedEventDelegate UserUpdated { get; set; } = null;
        public GuildUserEventDelegate UserJoinGuild { get; set; } = null;
        public GuildUserEventDelegate UserLeaveGuild { get; set; } = null;
        public GuildEventDelegate JoinedGuild { get; set; } = null;
        public GuildEventDelegate LeftGuild { get; set; } = null;

        public List<ICommandEvent> Events { get; set; } = new List<ICommandEvent>();
        public List<IService> Services { get; set; } = new List<IService>();

        private Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        internal EventSystem EventSystem;

        public string SqlName
        {
            get
            {
                return "module:" + Name;
            }
        }

        private bool isInstalled = false;

        internal RuntimeModule()
        {
        }

        public RuntimeModule(string name, bool enabled = true)
        {
            Name = name;
            Enabled = enabled;
        }

        public RuntimeModule(IModule info)
        {
            Name = info.Name;
            Enabled = info.Enabled;
            CanBeDisabled = info.CanBeDisabled;
            Events = info.Events;
        }

        public RuntimeModule(Action<IModule> info)
        {
            info.Invoke(this);
        }

        public async Task InstallAsync(object bot)
        {
            Bot b = (Bot)bot;
            Name = Name.ToLower();

            if (MessageRecieved != null)
            {
                b.MessageReceived += Module_MessageReceived;
            }

            if (UserUpdated != null)
            {
                b.UserUpdated += Module_UserUpdated;
            }

            if (UserJoinGuild != null)
            {
                b.UserJoin += Module_UserJoined;
            }

            if (UserLeaveGuild != null)
            {
                b.UserLeft += Module_UserLeft;
            }

            if (JoinedGuild != null)
            {
                b.GuildJoin += Module_JoinedGuild;
            }

            if (LeftGuild != null)
            {
                b.GuildLeave += Module_LeftGuild;
            }

            EventSystem = b.Events;

            b.Events.CommandHandler.Modules.Add(Name, this);

            foreach (ICommandEvent e in Events)
            {
                RuntimeCommandEvent ev = new RuntimeCommandEvent(e)
                {
                    eventSystem = b.Events,
                    Module = this
                };
                EventSystem.CommandHandler.AddCommand(ev);
            }

            isInstalled = true;

            await Task.CompletedTask;
        }

        public RuntimeModule AddCommand(ICommandEvent command)
        {
            Events.Add(command);
            return this;
        }

        public async Task UninstallAsync(object bot)
        {
            Bot b = (Bot)bot;

            if (!isInstalled)
            {
                return;
            }

            b.Events.Modules.Remove(Name);

            b.Events.CommandHandler.AddModule(this);

            if (MessageRecieved != null)
            {
                b.MessageReceived -= Module_MessageReceived;
            }

            if (UserUpdated != null)
            {
                b.UserUpdated -= Module_UserUpdated;
            }

            if (UserJoinGuild != null)
            {
                b.UserJoin -= Module_UserJoined;
            }

            if (UserLeaveGuild != null)
            {
                b.UserLeft -= Module_UserLeft;
            }

            if (JoinedGuild != null)
            {
                b.GuildJoin -= Module_JoinedGuild;
            }

            if (LeftGuild != null)
            {
                b.GuildLeave -= Module_LeftGuild;
            }

            isInstalled = false;
            await Task.CompletedTask;
        }

        private async Task Module_JoinedGuild(IDiscordGuild arg)
        {
            if (await IsEnabled(arg.Id))
            {
                try
                {
                    await JoinedGuild(arg);
                }
                catch (Exception e)
                {
                    Log.ErrorAt(Name, e.Message);
                }
            }
        }

        public RuntimeModule SetNsfw(bool val)
        {
            Nsfw = val;
            return this;
        }

        private async Task Module_LeftGuild(IDiscordGuild arg)
        {
            if (await IsEnabled(arg.Id))
            {
                await LeftGuild(arg);
            }
        }

        private async Task Module_UserJoined(IDiscordUser arg)
        {
            if (await IsEnabled(arg.Guild.Id))
            {
                await UserJoinGuild(arg.Guild, arg);
            }
        }

        private async Task Module_UserLeft(IDiscordUser arg)
        {
            if (await IsEnabled(arg.Guild.Id))
            {
                await UserLeaveGuild(arg.Guild, arg);
            }
        }

        private async Task Module_UserUpdated(IDiscordUser arg1, IDiscordUser arg2)
        {
            if (arg1.Guild != null)
            {
                if (await IsEnabled(arg1.Guild.Id))
                {
                    await UserUpdated(arg1, arg2);
                }
            }
        }

        private async Task Module_MessageReceived(IDiscordMessage message)
        {
            if (await IsEnabled(message.Guild.Id))
            {
                try
                {
                    Task.Run(() => MessageRecieved(message));
                }
                catch (Exception ex)
                {
                    Log.ErrorAt("module@message", ex.Message);
                }
            }
        }

        public async Task SetEnabled(ulong serverId, bool enabled)
        {
            if (this.enabled.ContainsKey(serverId))
            {
                this.enabled[serverId] = enabled;
            }
            else
            {
                this.enabled.Add(serverId, enabled);
            }

            using (var context = IAContext.CreateNoCache())
            {
                ModuleState state = await context.ModuleStates.FindAsync(SqlName, serverId.ToDbLong());
                if (state == null)
                {
                    state = context.ModuleStates.Add(new ModuleState() { ChannelId = serverId.ToDbLong(), ModuleName = SqlName, State = Enabled });
                }
                state.State = enabled;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            using (var context = IAContext.CreateNoCache())
            {
                long guildId = id.ToDbLong();
                ModuleState state = await context.ModuleStates.FindAsync(SqlName, guildId);
                if (state == null)
                {
                    return Enabled;
                }
                enabled.Add(id, state.State);
                return state.State;
            }
        }

        internal RuntimeModule Add(RuntimeModule module)
        {
            throw new NotImplementedException();
        }
    }
}