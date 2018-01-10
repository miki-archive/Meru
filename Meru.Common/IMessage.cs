using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common
{
    public interface IMessage : IEntity
    {
		IUser Author { get; }
		IChannel Channel { get; }
        string Content { get; }

		DateTimeOffset Timestamp { get; }

		Task<IMessage> ModifyAsync(string message, object embed = null);
		Task<IUser> GetSelfAsync();
    }
}
