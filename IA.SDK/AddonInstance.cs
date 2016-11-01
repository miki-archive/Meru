using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class AddonInstance
    {
        public List<ModuleInstance> modules = new List<ModuleInstance>();

        public void CreateModule(Action<ModuleData> x)
        {
            modules.Add(new ModuleInstance(x));
        }
    }
}
