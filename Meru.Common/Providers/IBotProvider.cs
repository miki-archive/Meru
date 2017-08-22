using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common.Providers
{
    public interface IBotProvider : IRunnable
    {
        event Func<IMessageObject, Task> OnMessageDelete;
        event Func<IMessageObject, Task> OnMessageEdit;
        event Func<IMessageObject, Task> OnMessageReceive;
    }
}
