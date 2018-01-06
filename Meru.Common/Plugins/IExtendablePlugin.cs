using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Common.Plugins
{
    public interface IExtendablePlugin : IPlugin
    {
        void AddPlugin(IPlugin plugin);
        void RemovePlugin(IPlugin plugin);

		T GetPluginOfType<T>() where T : IPlugin;
    }
}
