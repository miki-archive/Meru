using IA.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    class RuntimeAddonInstance : AddonInstance
    {
        public RuntimeAddonInstance()
        {

        }
        public RuntimeAddonInstance(AddonInstance i)
        {
            name = i.name;
            modules = i.modules;
        }

        public override async Task QueryAsync(string text, QueryOutput output, params object[] parameters)
        {
            await Sql.QueryAsync(text, output, parameters);
        }
    }
}
