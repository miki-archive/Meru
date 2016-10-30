using System.Collections.Generic;

namespace IA.SDK
{
    public class ModuleData
    {
        public string name;
        public bool enabled;

        public List<CommandEvent> events = new List<CommandEvent>();
    }
}