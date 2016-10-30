using System.Collections.Generic;

namespace IA.SDK
{
    public class ModuleInformation
    {
        public string name;
        public bool enabled;

        public List<CommandEvent> events = new List<CommandEvent>();
    }
}