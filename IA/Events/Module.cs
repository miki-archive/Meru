using Discord;
using Discord.WebSocket;
using IA.Database;
using IA.SDK;
using IA.SDK.Events;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IA.Events
{
    public class RuntimeModule : IModule
    {
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;

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
            MessageRecieved = info.MessageRecieved;
            UserUpdated = info.UserUpdated;
            UserJoinGuild = info.UserJoinGuild;
            UserLeaveGuild = info.UserLeaveGuild;
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
                b.Client.MessageReceived += Module_MessageRecieved;
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
            }

            if (MessageRecieved != null)
            {
                b.Client.MessageReceived -= Module_MessageRecieved;
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
                await JoinedGuild(r);
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

        private async Task Module_MessageRecieved(IMessage message)
        {
            RuntimeMessage msg = new RuntimeMessage(message);
            if (await IsEnabled(msg.Guild.Id))
            {
                try
                {
                    await MessageRecieved(msg);
                }
                catch (Exception ex)
                {
                    Log.ErrorAt("module@message", ex.Message);
                }
            }
        }

        public async Task SetEnabled(ulong serverid, bool enabled)
        {
            if (EventSystem.bot.SqlInformation != null)
            {
                if (this.enabled.ContainsKey(serverid))
                {
                    this.enabled[serverid] = enabled;
                }
                else
                {
                    this.enabled.Add(serverid, enabled);
                }
                await Sql.QueryAsync($"UPDATE event SET enabled=?enabled WHERE id=?id AND name=?name;", null, enabled, serverid, SqlName);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (EventSystem.bot.SqlInformation == null)
            {
                return Enabled;
            }

            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            int state = IsEventEnabled(id);
            if (state == -1)
            {
                await Sql.QueryAsync("INSERT INTO event(name, id, enabled) VALUES(?name, ?id, ?enabled);", null, SqlName, id, Enabled);
                enabled.Add(id, Enabled);
                return Enabled;
            }
            bool actualState = (state == 1) ? true : false;

            enabled.Add(id, actualState);
            return actualState;
        }

        // TODO: Query this.
        private int IsEventEnabled(ulong serverid)
        {
            if (EventSystem.bot.SqlInformation == null) return 1;

            MySqlConnection connection = new MySqlConnection(EventSystem.bot.SqlInformation.GetConnectionString());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT enabled FROM event WHERE id=\"{serverid}\" AND name=\"{SqlName}\"";

            connection.Open();
            MySqlDataReader r = command.ExecuteReader();

            bool output = false;
            string check = "";

            while (r.Read())
            {
                output = r.GetBoolean(0);
                check = "ok";
                break;
            }
            connection.Close();

            if (check == "")
            {
                return -1;
            }
            return output ? 1 : 0;
        }
    }
}