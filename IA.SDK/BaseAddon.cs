using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class BaseAddon
    {
        protected AddonInstance instance = new AddonInstance();

        public virtual void Create()
        {
            instance = new AddonInstance();
        }

        public AddonInstance GetModule()
        {
            return instance;
        }
    }
}
