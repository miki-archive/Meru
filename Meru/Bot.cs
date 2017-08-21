using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meru.Common;
using Meru.Common.Plugins;
using Meru.Common.Providers;

namespace Meru
{
    public partial class Bot : IBot
    {
        private readonly List<IBotProvider> providers = new List<IBotProvider>();
        private readonly List<IPlugin> plugins = new List<IPlugin>();

        private List<IRunnable> allRunnables
        { 
            get
            {
                List<IRunnable> runnables = new List<IRunnable>();
                runnables.AddRange(providers);
                runnables.AddRange(plugins);

                return runnables;
            }
        }

        public void AddPlugin(IPlugin plugin)
        {
            plugins.Add(plugin);
            Console.WriteLine($"Added plugin {plugin.GetType().Name}");
        }

        public void AddProvider(IBotProvider provider)
        {
            provider.OnMessageReceived += async (m) =>
            {
                await OnMessageReceived.Invoke(m);
            };

            providers.Add(provider);
            Console.WriteLine($"Added provider {provider.GetType().Name}");
        }

        public async Task StartAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StartAsync();
            }
        }

        public async Task StopAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StopAsync();
            }
        }
    }
}
