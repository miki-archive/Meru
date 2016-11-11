﻿using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Module
    {
        public ModuleInformation defaultInfo = new ModuleInformation();

        Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        bool isInstalled = false;

        public Module()
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
        public Module(SDK.ModuleInstance addon)
        {
            defaultInfo = new ModuleInformation();
            defaultInfo.name = addon.data.name;
            defaultInfo.enabled = addon.data.enabled;
            defaultInfo.events = new List<CommandEvent>();
            foreach (SDK.CommandEvent e in addon.data.events)
            {
                defaultInfo.events.Add(new CommandEvent(x =>
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
                bot.Client.MessageReceived += Client_MessageReceived;
            }

            foreach (CommandEvent e in defaultInfo.events)
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
            if(defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived += Client_MessageReceived;
            }

            foreach(CommandEvent e in defaultInfo.events)
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

        public Task UninstallAsync(Bot bot)
        {
            if (!isInstalled)
            {
                return Task.CompletedTask;
            }

            foreach (CommandEvent e in defaultInfo.events)
            {
                defaultInfo.eventSystem.events.CommandEvents.Remove(e.name);
            }

            if (defaultInfo.messageEvent != null)
            {
                bot.Client.MessageReceived -= Client_MessageReceived;
            }

            isInstalled = false;
            return Task.CompletedTask;
        }

        private async Task Client_MessageReceived(IMessage message)
        {
            await defaultInfo.messageEvent(message);
        }
    }
}
