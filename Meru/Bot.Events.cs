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

        public event Func<IBotProvider, Task> OnProviderConnect;
        public event Func<IBotProvider, Task> OnProviderDisconnect;
    }
}
