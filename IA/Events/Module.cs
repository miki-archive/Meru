using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class Module
    {
        public ModuleInformation defaultInfo;

        Dictionary<ulong, bool> enabled = new Dictionary<ulong, bool>();

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

        public void Load(ulong channelId)
        {

        }

        public void SetActive(ulong channelId, bool value)
        {
            if(!enabled.ContainsKey(channelId))
            {
                Load(channelId);
            }
        }
    }
}
