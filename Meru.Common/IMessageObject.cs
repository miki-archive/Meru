using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common
{
    public interface IMessageObject : IEntityObject
    {
        IUserObject Author { get; }
        IChannelObject Channel { get; }

        string Content { get; }

        Task ModifyAsync(string message);
    }
}
