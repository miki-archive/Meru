using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class ModuleInstance
    {
        public ModuleData data = new ModuleData();

        Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        public ModuleInstance(string name, bool enabled = true)
        {
            data = new ModuleData();
            data.name = name;
            data.enabled = enabled;
        }       
        public ModuleInstance(Action<ModuleData> info)
        {
            info.Invoke(data);
        }

        public Task AddCommand(Action<CommandEvent> x)
        {
            CommandEvent y = new CommandEvent(x);
            data.events.Add(y);
            return Task.CompletedTask;
        }

        public string GetState()
        {
            return data.name + ": " + "ACTIVE";
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public Task Install()
        {
            return Task.CompletedTask;
        }

        public Task Uninstall()
        {
            return Task.CompletedTask;
        }
    }
}
