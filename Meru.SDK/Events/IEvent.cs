using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Events
{
    public interface IEvent
    {
        string Name { get; set; }
        string[] Aliases { get; set; }

        EventAccessibility Accessibility { get; set; }

        bool OverridableByDefaultPrefix { get; set; }
        bool CanBeDisabled { get; set; }
        bool DefaultEnabled { get; set; }

        IModule Module { get; set; }

        int TimesUsed { get; set; }

        Task<bool> IsEnabled(ulong id);

        Task SetEnabled(ulong id, bool value);
        Task SetEnabledAll(IDiscordGuild guild, bool value);

        IEvent SetName(string name);
        IEvent SetAliases(params string[] aliases);
        IEvent SetAccessibility(EventAccessibility accessibility);
    }
}
