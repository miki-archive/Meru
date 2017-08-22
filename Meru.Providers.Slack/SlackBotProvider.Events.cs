using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;

namespace Meru.Providers.Slack
{
    public partial class SlackBotProvider
    {
        public event Func<IMessageObject, Task> OnMessageReceived;
    }
}
