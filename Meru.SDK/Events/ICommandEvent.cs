using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Events
{
    public interface ICommandEvent : IEvent
    {
        Dictionary<string, ProcessCommandDelegate> CommandPool { get; set; }
        int Cooldown { get; set; }

        List<DiscordGuildPermission> GuildPermissions { get; set; }

        CheckCommandDelegate CheckCommand { get; set; }
        ProcessCommandDelegate ProcessCommand { get; set; }

        Task Check(IDiscordMessage e, ICommandHandler c, string identifier = "");

        new ICommandEvent SetName(string name);
        new ICommandEvent SetAccessibility(EventAccessibility accessibility);
        new ICommandEvent SetAliases(params string[] aliases);
        ICommandEvent SetCooldown(int seconds);
        ICommandEvent SetPermissions(params DiscordGuildPermission[] permissions);
        ICommandEvent On(string args, ProcessCommandDelegate command);
        ICommandEvent Default(ProcessCommandDelegate command);
    }
}
