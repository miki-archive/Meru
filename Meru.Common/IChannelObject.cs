using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common
{
    public interface IChannelObject : IEntityObject
    {
        IGuildObject Guild { get; }

        Task<IMessageObject> SendMessageAsync(string message);
    }
}
