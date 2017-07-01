using IA.Models;
using IA.Models.Context;
using IA.SDK;
using IA.SDK.Events;
using IA.SDK.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Event : IEvent
    {
        public string Name { get; set; } = "$command-not-named";
        public string[] Aliases { get; set; } = new string[] { };

        public EventAccessibility Accessibility { get; set; } = EventAccessibility.PUBLIC;
        public EventMetadata Metadata { get; set; } = new EventMetadata();

        public bool OverridableByDefaultPrefix { get; set; } = false;
        public bool CanBeDisabled { get; set; } = true;
        public bool DefaultEnabled { get; set; } = true;
  
        public IModule Module { get; set; }

        public int TimesUsed { get; set; } = 0;

        internal EventSystem eventSystem;

        public Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();
        protected Dictionary<ulong, DateTime> lastTimeUsed = new Dictionary<ulong, DateTime>();

        public Event() { }
        public Event(IEvent eventObject)
        {
            Name = eventObject.Name;
            Aliases = eventObject.Aliases;
            Accessibility = eventObject.Accessibility;
            OverridableByDefaultPrefix = eventObject.OverridableByDefaultPrefix;
            CanBeDisabled = eventObject.CanBeDisabled;
            DefaultEnabled = eventObject.DefaultEnabled;
            Module = eventObject.Module;
            TimesUsed = eventObject.TimesUsed;
        }
        public Event(Action<Event> info)
        {
            info.Invoke(this);
        }

        public async Task SetEnabled(ulong channelId, bool enabled)
        {
            if (this.enabled.ContainsKey(channelId))
            {
                this.enabled[channelId] = enabled;
            }
            else
            {
                this.enabled.Add(channelId, enabled);
            }

            using (var context = IAContext.CreateNoCache())
            {
                CommandState state = await context.CommandStates.FindAsync(Name, channelId.ToDbLong());
                if (state == null)
                {
                    state = context.CommandStates.Add(new CommandState() { ChannelId = channelId.ToDbLong(), CommandName = Name, State = DefaultEnabled });
                }
                state.State = enabled;
                await context.SaveChangesAsync();
            }
        }
        public async Task SetEnabledAll(IDiscordGuild guildId, bool enabled)
        {
            List<IDiscordMessageChannel> channels = await guildId.GetChannels();
            foreach(IDiscordMessageChannel c in channels)
            {
                await SetEnabled(c.Id, enabled);
            }
        }

        public async Task<bool> IsEnabled(ulong id)
        {
            if (Module != null)
            {
                if (!await Module.IsEnabled(id)) return false;
            }

            if (enabled.ContainsKey(id))
            {
                return enabled[id];
            }

            using (var context = IAContext.CreateNoCache())
            {
                CommandState state = await context.CommandStates.FindAsync(Name, id.ToDbLong());
                if (state == null)
                {
                    return DefaultEnabled;
                }
                return state.State;
            }
        }

        public IEvent SetName(string name)
        {
            Name = name;
            return this;
        }

        public IEvent SetAliases(params string[] aliases)
        {
            Aliases = aliases;
            return this;
        }

        public IEvent SetAccessibility(EventAccessibility accessibility)
        {
            Accessibility = accessibility;
            return this;
        }
    }
}