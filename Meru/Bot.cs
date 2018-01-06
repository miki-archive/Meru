using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meru.Common;
using Meru.Common.Plugins;
using Meru.Common.Providers;

namespace Meru
{
    public partial class Bot : BaseExtendablePlugin, IBot
    {
        public bool IsRunning { get; private set; }

        private readonly List<IBotProvider> providers = new List<IBotProvider>();

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

        public void AddProvider(IBotProvider provider)
        {
			provider.OnMessageReceive += MessageReceive;
            provider.OnMessageEdit += async (m) =>
            {
                await OnMessageEdit?.Invoke(m);
            };
            provider.OnMessageDelete += async (m) =>
            {
                await OnMessageDelete?.Invoke(m);
            };

            providers.Add(provider);
        }

        public override async Task StartAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StartAsync();
            }
        }

        public override async Task StopAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StopAsync();
            }
        }

		private async Task MessageReceive(IMessage msg)
		{
			try
			{
				await OnMessageReceive.Invoke(msg);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
