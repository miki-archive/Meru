using Discord;
using Discord.WebSocket;
using IA.SDK;
using IA.SQL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Module
    {
        public ModuleInformation defaultInfo = new ModuleInformation();

        private Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        public string SqlName
        {
            get
            {
                return "module:" + defaultInfo.name;
            }
        }

        private bool isInstalled = false;

        internal Module()
        {
        }

        public Module(string name, bool enabled = true)
        {
            defaultInfo = new ModuleInformation();
            defaultInfo.name = name;
            defaultInfo.enabled = enabled;
        }

        public Module(Action<ModuleInformation> info)
        {
            info.Invoke(defaultInfo);
        }

        public Module(ModuleInstance addon)
        {
            defaultInfo = new ModuleInformation();
            defaultInfo.name = addon.data.name.ToLower();
            defaultInfo.enabled = addon.data.enabled;

            defaultInfo.messageEvent = addon.data.messageRecieved;
            defaultInfo.userUpdateEvent = addon.data.userUpdated;

            defaultInfo.guildJoinEvent = addon.data.userJoin;
            defaultInfo.guildLeaveEvent = addon.data.userLeave;

            defaultInfo.events = new List<RuntimeCommandEvent>();
            foreach (CommandEvent e in addon.data.events)
            {
                defaultInfo.events.Add(new RuntimeCommandEvent(x =>
                {
                    x.name = e.name;
                    x.module = this;
                    if (e.processCommand != null)
                    {
                        x.processCommand = e.processCommand;
                    }
                    x.accessibility = e.accessibility;
                    x.requiresPermissions = e.requiresPermissions;
                    x.usage = e.metadata.usage.ToArray();
                    if (e.checkCommand != null)
                    {
                        x.checkCommand = e.checkCommand;
                    }
                    x.aliases = e.aliases;
                    x.canBeDisabled = e.canBeDisabled;
                    x.canBeOverridenByDefaultPrefix = e.canBeOverridenByDefaultPrefix;
                    x.cooldown = e.cooldown;
                    x.description = e.metadata.description;
                    x.errorMessage = e.metadata.errorMessage;
                    x.defaultEnabled = e.defaultEnabled;
                }));
            }
        }

        public string GetState()
        {
            return defaultInfo.name + ": " + (isInstalled ? "Installed" : "Uninstalled");
        }

        // TODO: either remove or use this for something.
        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public async Task InstallAsync(Bot bot)
        {
            defaultInfo.name = defaultInfo.name.ToLower();
            bot.Events.Modules.Add(defaultInfo.name, this);

            if (defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived += Module_MessageRecieved;
            }

            if (defaultInfo.userUpdateEvent != null)
            {
                bot.Client.UserUpdated += Module_UserUpdated;
            }

            if (defaultInfo.guildJoinEvent != null)
            {
                bot.Client.UserJoined += Module_UserJoined;
            }

            if (defaultInfo.guildLeaveEvent != null)
            {
                bot.Client.UserLeft += Module_UserLeft;
            }

            defaultInfo.eventSystem = bot.Events;

            foreach (RuntimeCommandEvent e in defaultInfo.events)
            {
                e.eventSystem = bot.Events;
                defaultInfo.eventSystem.events.CommandEvents.Add(e.name, e);
            }

            isInstalled = true;

            await Task.CompletedTask;
        }

        public async Task UninstallAsync(Bot bot)
        {
            if (!isInstalled)
            {
                return;
            }

            bot.Events.Modules.Remove(defaultInfo.name);

            foreach (RuntimeCommandEvent e in defaultInfo.events)
            {
                defaultInfo.eventSystem.events.CommandEvents.Remove(e.name);
            }

            if (defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived -= Module_MessageRecieved;
            }

            if (defaultInfo.userUpdateEvent != null)
            {
                bot.Client.UserUpdated -= Module_UserUpdated;
            }

            if (defaultInfo.guildJoinEvent != null)
            {
                bot.Client.UserJoined -= Module_UserJoined;
            }

            if (defaultInfo.guildLeaveEvent != null)
            {
                bot.Client.UserLeft -= Module_UserLeft;
            }

            isInstalled = false;
            await Task.CompletedTask;
        }

        private async Task Module_UserJoined(SocketGuildUser arg)
        {
            RuntimeUser r = new RuntimeUser(arg);

            if (await IsEnabled(r.Guild.Id))
            {
                await defaultInfo.guildJoinEvent(r.Guild, r);
            }
        }

        private async Task Module_UserLeft(SocketGuildUser arg)
        {
            RuntimeUser r = new RuntimeUser(arg);

            if (await IsEnabled(r.Guild.Id))
            {
                await defaultInfo.guildLeaveEvent(r.Guild, r);
            }
        }

        private async Task Module_UserUpdated(SocketUser arg1, SocketUser arg2)
        {
            RuntimeUser usr1 = new RuntimeUser(arg1);
            RuntimeUser usr2 = new RuntimeUser(arg2);
            if (await IsEnabled(usr1.Guild.Id))
            {
                await defaultInfo.userUpdateEvent(usr1, usr2);
            }
        }

        private async Task Module_MessageRecieved(IMessage message)
        {
            RuntimeMessage msg = new RuntimeMessage(message);
            if (await IsEnabled(msg.Guild.Id))
            {
                try
                {
                    Task.Run(async () =>
                    {
                        Stopwatch s = new Stopwatch();
                        s.Start();
                        await defaultInfo.messageEvent(msg);
                        s.Stop();
                    });
                }
                catch (Exception ex)
                {
                    Log.ErrorAt("module@message", ex.Message);
                }
            }
        }

        public async Task SetEnabled(ulong serverid, bool enabled)
        {
            if (defaultInfo.eventSystem.bot.SqlInformation != null)
            {
                if (this.enabled.ContainsKey(serverid))
                {
                    this.enabled[serverid] = enabled;
                }
                else
                {
                    this.enabled.Add(serverid, enabled);
                }
                await MySQL.QueryAsync($"UPDATE event SET enabled=?enabled WHERE id=?id AND name=?name;", null, enabled, serverid, SqlName);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (defaultInfo.eventSystem.bot.SqlInformation == null)
            {
                return defaultInfo.enabled;
            }

            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            int state = IsEventEnabled(id);
            if (state == -1)
            {
                await MySQL.QueryAsync("INSERT INTO event(name, id, enabled) VALUES(?name, ?id, ?enabled);", null, SqlName, id, defaultInfo.enabled);
                enabled.Add(id, defaultInfo.enabled);
                return defaultInfo.enabled;
            }
            bool actualState = (state == 1) ? true : false;

            enabled.Add(id, actualState);
            return actualState;
        }

        // TODO: Query this.
        private int IsEventEnabled(ulong serverid)
        {
            if (defaultInfo.eventSystem.bot.SqlInformation == null) return 1;

            MySqlConnection connection = new MySqlConnection(defaultInfo.eventSystem.bot.SqlInformation.GetConnectionString());
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