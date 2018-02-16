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

		/// <summary>
		/// Add a provider for the bot to listen to
		/// </summary>
		/// <param name="provider">a provider wrapped into IBotProvider</param>
        public void AddProvider(IBotProvider provider)
        {
			provider.OnMessageReceive += OnMessageReceive;
			provider.OnMessageEdit += OnMessageEdit;
			provider.OnMessageDelete += OnMessageDelete;

            providers.Add(provider);
        }

		/// <summary>
		/// Starts the bot and all modules
		/// </summary>
        public override async Task StartAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StartAsync();
            }

			await base.StartAsync();
		}

		/// <summary>
		/// Stops the bot and all modules
		/// </summary>
        public override async Task StopAsync()
        {
            foreach (IRunnable runnable in allRunnables)
            {
                await runnable.StopAsync();
            }

			await base.StopAsync();
        }
	}

	/// <summary>
	/// Events
	/// </summary>
	public partial class Bot
	{
		public event Func<Task> OnBotStart;
		public event Func<Task> OnBotStop;

		public event Func<IMessage, Task> OnMessageDelete;
		public event Func<IMessage, Task> OnMessageEdit;
		public event Func<IMessage, Task> OnMessageReceive;

		public event Func<IGuild, Task> OnGuildCreate;
		public event Func<IGuild, Task> OnGuildUpdate;
		public event Func<IUser, Task> OnGuildMemberAdd;
		public event Func<IUser, Task> OnGuildMemberRemove;
		public event Func<IUser, Task> OnGuildMemberUpdate;
		public event Func<IUser, Task> OnUserUpdate;

		public event Func<IBotProvider, Task> OnProviderConnect;
		public event Func<IBotProvider, Task> OnProviderDisconnect;
	}
}
