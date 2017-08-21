using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common.Plugins
{
    public interface IPlugin : IRunnable
    {
        IBot AttachedBot { get; set; }
    }
}
