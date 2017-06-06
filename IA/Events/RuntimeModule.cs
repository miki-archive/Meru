using Discord;
using Discord.WebSocket;
using IA.Models;
using IA.Models.Context;
using IA.SDK;
using IA.SDK.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeModule : IModule
    {
        public string Name { get; set; } = "";
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

        internal RuntimeModule() { }
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
            MessageRecieved = info.MessageRecieved;
            UserUpdated = info.UserUpdated;
            UserJoinGuild = info.UserJoinGuild;
            UserLeaveGuild = info.UserLeaveGuild;
            JoinedGuild = info.JoinedGuild;
            LeftGuild = info.LeftGuild;
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

            b.Events.Modules.Add(Name, this);

            if (MessageRecieved != null)
            {
                b.Client.MessageReceived += Module_MessageReceived;
            }

            if (UserUpdated != null)
            {
                b.Client.UserUpdated += Module_UserUpdated;
            }

            if (UserJoinGuild != null)
            {
                b.Client.UserJoined += Module_UserJoined;
            }

            if (UserLeaveGuild != null)
            {
                b.Client.UserLeft += Module_UserLeft;
            }

            if (JoinedGuild != null)
            {
                b.Client.JoinedGuild += Module_JoinedGuild;
            }

            if (LeftGuild != null)
            {
                b.Client.LeftGuild += Module_LeftGuild;
            }

            EventSystem = b.Events;

            foreach (ICommandEvent e in Events)
            {
                RuntimeCommandEvent ev = new RuntimeCommandEvent(e);
                ev.eventSystem = b.Events;
                ev.Module = this;

                foreach(string alias in e.Aliases)
                {
                    ev.eventSystem.aliases.Add(alias.ToLower(), ev.Name.ToLower());
                }

                EventSystem.events.CommandEvents.Add(ev.Name, ev);
            }

            isInstalled = true;

            await Task.CompletedTask;
        }

        public async Task UninstallAsync(object bot)
        {
            Bot b = (Bot)bot;

            if (!isInstalled)
            {
                return;
            }

            b.Events.Modules.Remove(Name);

            foreach (ICommandEvent e in Events)
            {
                ICommandEvent ev = new RuntimeCommandEvent(e);
                EventSystem.events.CommandEvents.Remove(ev.Name);
                foreach (string x in e.Aliases)
                {
                    EventSystem.aliases.Add(x.ToLower(), Name.ToLower());
                }
            }

            if (MessageRecieved != null)
            {
                b.Client.MessageReceived -= Module_MessageReceived;
            }

            if (UserUpdated != null)
            {
                b.Client.UserUpdated -= Module_UserUpdated;
            }

            if (UserJoinGuild != null)
            {
                b.Client.UserJoined -= Module_UserJoined;
            }

            if (UserLeaveGuild != null)
            {
                b.Client.UserLeft -= Module_UserLeft;
            }

            if (JoinedGuild != null)
            {
                b.Client.JoinedGuild -= Module_JoinedGuild;
            }

            if (LeftGuild != null)
            {
                b.Client.LeftGuild -= Module_LeftGuild;
            }

            isInstalled = false;
            await Task.CompletedTask;
        }

        private async Task Module_JoinedGuild(SocketGuild arg)
        {
            RuntimeGuild r = new RuntimeGuild(arg);

            if (await IsEnabled(r.Id))
            {
                try
                {
                    await JoinedGuild(r);
                }
                catch (Exception e)
                {

                }
            }
        }

        private async Task Module_LeftGuild(SocketGuild arg)
        {
            RuntimeGuild r = new RuntimeGuild(arg);

            if (await IsEnabled(r.Id))
            {
                await LeftGuild(r);
            }
        }

        private async Task Module_UserJoined(SocketGuildUser arg)
        {
            RuntimeUser r = new RuntimeUser(arg);

            if (await IsEnabled(r.Guild.Id))
            {
                await UserJoinGuild(r.Guild, r);
            }
        }

        private async Task Module_UserLeft(SocketGuildUser arg)
        {
            RuntimeUser r = new RuntimeUser(arg);

            if (await IsEnabled(r.Guild.Id))
            {
                await UserLeaveGuild(r.Guild, r);
            }
        }

        private async Task Module_UserUpdated(SocketUser arg1, SocketUser arg2)
        {
            RuntimeUser usr1 = new RuntimeUser(arg1);
            RuntimeUser usr2 = new RuntimeUser(arg2);
            if (await IsEnabled(usr1.Guild.Id))
            {
                await UserUpdated(usr1, usr2);
            }
        }

        private async Task Module_MessageReceived(IMessage message)
        {
            RuntimeMessage msg = new RuntimeMessage(message);
            if (await IsEnabled(msg.Guild.Id))
            {
                try
                {
                    Task.Run(() => MessageRecieved(msg));
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
                ModuleState state = await context.ModuleStates.FindAsync(Name, serverId.ToDbLong());
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
    }
}