using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common.Plugins;

namespace Meru.Common
{
    public interface IBot : IRunnable
    {
        event Func<IMessageObject, Task> OnMessageReceived;
    }
}
