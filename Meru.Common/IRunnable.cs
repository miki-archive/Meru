using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Common
{
    public interface IRunnable
    {
        bool IsRunning { get; }

        Task StartAsync();
        Task StopAsync();
    }
}
