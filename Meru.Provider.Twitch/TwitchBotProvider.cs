using System;
using System.Threading.Tasks;
using Meru.Common;
using Meru.Common.Providers;
using NTwitch;

namespace Meru.Providers.Twitch
{
    public partial class TwitchBotProvider : IBotProvider
    {
        public event Func<IMessageObject, Task> OnMessageReceived;   

        public Task StartAsync()
        {      
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
