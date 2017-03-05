using IA.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.SDK.Interfaces;
using IA.SDK.Events;
using IA.Events;

namespace IA.SDK
{
    class RuntimeAddonInstance : IAddonInstance
    {
        public string Name { get; set; } = "";
        public List<IModule> Modules { get; set; } = new List<IModule>();

        public RuntimeAddonInstance()
        {
        }
        public RuntimeAddonInstance(IAddonInstance i)
        {
            Name = i.Name;
            Modules = i.Modules;
        }

        public async Task QueryAsync(string text, QueryOutput output, params object[] parameters)
        {
            await Sql.QueryAsync(text, output, parameters);
        }

        public string GetBotVersion()
        {
            return Bot.instance.Version;
        }

        public async Task<string> ListCommands(IDiscordMessage e)
        {
            return await Bot.instance.Events.ListCommands(e);
        }

        public ICommandEvent GetCommandEvent(string args)
        {
            return Bot.instance.Events.GetCommandEvent(args);
        }

        public void CreateModule(Action<IModule> module)
        {
            IModule m = new RuntimeModule(module);
        }

        public EventAccessibility GetUserAccessibility(IDiscordMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetIdentifierAsync(ulong serverid)
        {
            throw new NotImplementedException();
        }

        public List<IModule> GetModules()
        {
            throw new NotImplementedException();
        }

        public Task SetIdentifierAsync(IDiscordGuild guild, string identifier)
        {
            throw new NotImplementedException();
        }
    }
}
