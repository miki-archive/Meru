using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common.Plugins
{
    public class BaseExtendablePlugin : IExtendablePlugin
    {
        public bool IsRunning { get; private set; } = false;

        public IBot AttachedBot { get; set; }

		// TODO: change to map?
        protected List<IPlugin> plugins { get; set; } = new List<IPlugin>();

        public void AddPlugin(IPlugin plugin)
        {
            plugins.Add(plugin);
        }

        public void RemovePlugin(IPlugin plugin)
        {
            if (plugin.IsRunning)
            {
                plugin.StopAsync();
            }

            plugins.Remove(plugin);
        }

        public virtual async Task StartAsync()
        {
            foreach (IPlugin p in plugins)
            {
                await p.StartAsync();
            }
            IsRunning = true;
        }

        public virtual async Task StopAsync()
        {
            foreach (IPlugin p in plugins)
            {
                await p.StopAsync();
            }
            IsRunning = false;
        }

		public virtual T GetPluginOfType<T>() where T : IPlugin
		{
			foreach(var plugin in plugins)
			{
				T t = (T)plugin;
				if(t != null)
				{
					return t;
				}
			}
			return default(T);
		}
    }
}
