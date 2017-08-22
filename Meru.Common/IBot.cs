using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common.Plugins;
using Meru.Common.Providers;

namespace Meru.Common
{
    public interface IBot : IBotProvider
    {
        event Func<Task> OnBotStart;
        event Func<Task> OnBotStop;
        
        event Func<IBotProvider, Task> OnProviderConnect;
        event Func<IBotProvider, Task> OnProviderDisconnect;
    }
}
