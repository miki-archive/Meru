using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common
{
    public interface IChannel : IEntity
    {
        IGuild Guild { get; }

        Task<IMessage> SendMessageAsync(string message, object embed = null);
    }
}
