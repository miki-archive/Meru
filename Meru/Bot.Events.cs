using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;
using Meru.Common.Providers;

namespace Meru
{
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
