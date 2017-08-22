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

        public event Func<IMessageObject, Task> OnMessageDelete;
        public event Func<IMessageObject, Task> OnMessageEdit; 
        public event Func<IMessageObject, Task> OnMessageReceive;

        public event Func<IBotProvider, Task> OnProviderConnect;
        public event Func<IBotProvider, Task> OnProviderDisconnect;
    }
}
