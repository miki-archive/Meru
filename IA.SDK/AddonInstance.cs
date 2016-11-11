using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class AddonInstance
    {
        public List<ModuleInstance> modules;

        public AddonInstance()
        {
            modules = new List<ModuleInstance>();
        }

        public void AddCommandEventTo(ModuleInstance selectedModule, Action<CommandEvent> command)
        {
            selectedModule.AddCommand(command);
        }

        public void CreateModule(Action<ModuleData> x)
        {
            modules.Add(new ModuleInstance(x));
        }
    }
}
