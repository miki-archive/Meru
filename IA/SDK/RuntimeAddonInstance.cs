using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meru.SDK.Interfaces;
using Meru.SDK.Events;
using Meru.Events;
using Discord;

namespace Meru.SDK
{
    class RuntimeAddonInstance : IAddonInstance
    {
        public string Name { get; set; } = "";
        public List<IModule> Modules { get; set; } = new List<IModule>();
        DiscordClient bot = null;

        public RuntimeAddonInstance()
        {
        }
        public RuntimeAddonInstance(IAddonInstance i, DiscordClient bot)
        {
            Name = i.Name;
            Modules = i.Modules;
            this.bot = bot;
        }

        public string GetBotVersion()
        {
            return DiscordClient.Instance.Version;
        }

        public async Task<string> ListCommands(IDiscordMessage e)
        {
            return await DiscordClient.Instance.Events.ListCommands(e);
        }
        public async Task<IDiscordEmbed> ListCommandsInEmbed(IDiscordMessage e)
        {
            return await DiscordClient.Instance.Events.ListCommandsInEmbed(e);
        }

        public ICommandEvent GetCommandEvent(string args)
        {
            return DiscordClient.Instance.Events.GetCommandEvent(args);
        }

        public void CreateModule(Action<IModule> module)
        {
            IModule m = new RuntimeModule(module);
            Modules.Add(m);
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage message)
        {
            return DiscordClient.Instance.Events.GetUserAccessibility(message);
        }

        public async Task<string> GetIdentifierAsync(ulong serverid)
        {
            return await DiscordClient.Instance.Events.GetIdentifier(serverid, PrefixInstance.Default);
        }

        public List<IModule> GetModules()
        {
            return DiscordClient.Instance.Events.Modules.Values.ToList();
        }

        public async Task SetIdentifierAsync(IDiscordGuild guild, string defaultPrefix, string newPrefix)
        {
            await DiscordClient.Instance.Events
                .GetPrefixInstance(defaultPrefix)
                .ChangeForGuildAsync(guild.Id, newPrefix);
        }

        public int GetGuildCount()
        {
            return DiscordClient.Instance.Client.Guilds.Count;
        }

        public ulong GetBotId(IDiscordGuild guild)
        {
            DiscordClient b = DiscordClient.Instance;
            ulong id = b.Client.GetShardFor((guild as IProxy<IGuild>).ToNativeObject()).CurrentUser.Id;

            return id;
        }
    }
}
