using Discord;
using IA.SDK;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace IA.Events
{
    public class Module
    {
        public ModuleInformation defaultInfo = new ModuleInformation();

        Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        bool isInstalled = false;

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
            defaultInfo.name = addon.data.name;
            defaultInfo.enabled = addon.data.enabled;
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
                    x.requiresPermissions = e.requiresPermissions;
                    x.usage = e.usage;
                    if (e.checkCommand != null)
                    {
                        x.checkCommand = e.checkCommand;
                    }
                    x.aliases = e.aliases;
                    x.canBeDisabled = e.canBeDisabled;
                    x.canBeOverridenByDefaultPrefix = e.canBeOverridenByDefaultPrefix;
                    x.cooldown = e.cooldown;
                    x.description = e.description;
                    x.errorMessage = e.errorMessage;
                    x.defaultEnabled = e.defaultEnabled;
                }));
            }
        }

        public string GetState()
        {
            return defaultInfo.name + ": " + "ACTIVE";
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public void Install(Bot bot)
        {
            if (defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived += Module_MessageRecieved;
            }

            foreach (RuntimeCommandEvent e in defaultInfo.events)
            {
                if (defaultInfo.eventSystem == null)
                {
                    defaultInfo.eventSystem = bot.Events;
                }
                defaultInfo.eventSystem.events.CommandEvents.Add(e.name, e);
            }

            isInstalled = true;
        }

        public async Task InstallAsync(Bot bot)
        {
            if(bot.isManager)
            {
                return;
            }

            if(defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived += Module_MessageRecieved;
            }

            if(defaultInfo.userUpdateEvent != null)
            {
                bot.Client.UserUpdated += Module_UserUpdated;
            }

            foreach(RuntimeCommandEvent e in defaultInfo.events)
            {
                if(defaultInfo.eventSystem == null)
                {
                    defaultInfo.eventSystem = bot.Events;
                }
                defaultInfo.eventSystem.events.CommandEvents.Add(e.name, e);
            }

            isInstalled = true;
            await Task.CompletedTask;
        }

        public async Task UninstallAsync(Bot bot)
        {
            if (!isInstalled || bot.isManager)
            {
                return;
            }

            foreach (RuntimeCommandEvent e in defaultInfo.events)
            {
                defaultInfo.eventSystem.events.CommandEvents.Remove(e.name);
            }

            if (defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived -= Module_MessageRecieved;
            }
            if(defaultInfo.userUpdateEvent != null)
            {
                bot.Client.UserUpdated -= Module_UserUpdated;
            }

            isInstalled = false;
            await Task.CompletedTask;
        }

        private async Task Module_UserUpdated(SocketUser arg1, SocketUser arg2)
        {
            RuntimeUser usr1 = new RuntimeUser(arg1);
            RuntimeUser usr2 = new RuntimeUser(arg2);
            await defaultInfo.userUpdateEvent(usr1, usr2);
        }

        private async Task Module_MessageRecieved(IMessage message)
        {
            RuntimeMessage msg = new RuntimeMessage(message);
            await defaultInfo.messageEvent(msg);
        }

        private async Task Module_GuildJoin(IGuild guild)
        {
            RuntimeGuild g = new RuntimeGuild(guild);
            await defaultInfo.guildJoinEvent.processCommand(g);   
        }

        private async Task Module_GuildLeave(IGuild guild)
        {
            RuntimeGuild g = new RuntimeGuild(guild);
            await defaultInfo.guildLeaveEvent.processCommand(g);
        }
    }
}
