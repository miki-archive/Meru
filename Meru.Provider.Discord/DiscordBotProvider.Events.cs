using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;
using Meru.Common.Providers;

namespace Meru.Providers.Discord
{
    public partial class DiscordBotProvider
    {
        public event Func<IMessageObject, Task> OnMessageReceived;
    }
}
