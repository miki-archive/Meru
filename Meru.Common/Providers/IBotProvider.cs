using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common.Providers
{
    public interface IBotProvider : IRunnable
    {
        event Func<IMessage, Task> OnMessageDelete;
        event Func<IMessage, Task> OnMessageEdit;
        event Func<IMessage, Task> OnMessageReceive;

		event Func<IGuild, Task> OnGuildCreate;
		event Func<IGuild, Task> OnGuildUpdate;

		event Func<IUser, Task> OnGuildMemberAdd;
		event Func<IUser, Task> OnGuildMemberRemove;
		event Func<IUser, Task> OnGuildMemberUpdate;

		event Func<IUser, Task> OnUserUpdate;
	}
}
