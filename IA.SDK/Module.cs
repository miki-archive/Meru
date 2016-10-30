using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class ModuleInstance
    {
        public ModuleData defaultInfo = new ModuleData();

        Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

        public ModuleInstance(string name, bool enabled = true)
        {
            defaultInfo = new ModuleData();
            defaultInfo.name = name;
            defaultInfo.enabled = enabled;
        }       
        public ModuleInstance(Action<ModuleData> info)
        {
            info.Invoke(defaultInfo);
        }

        public string GetState()
        {
            return defaultInfo.name + ": " + "ACTIVE";
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
