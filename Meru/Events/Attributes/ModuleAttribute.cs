using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events.Attributes
{
    public class ModuleAttribute : Attribute
    {
        internal RuntimeModule module = new RuntimeModule();

        public string Name
        {
            get => module.Name;
            set => module.Name = value;
        }

        public bool Nsfw
        {
            get => module.Nsfw;
            set => module.Nsfw = value;
        }

        public ModuleAttribute()
        {

        }
        public ModuleAttribute(string Name)
        {
            module.Name = Name;
        }
    }
}
