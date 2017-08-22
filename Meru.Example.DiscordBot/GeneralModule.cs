using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Commands;
using Meru.Common;

namespace Meru.Example.DiscordBot
{
    [Module(Name = "general")]
    class GeneralModule
    {
        [Command(Name = "ping")]
        public async Task PingAsync(IMessageObject message)
        {
            Task<IMessageObject> sendMessage = message.Channel.SendMessageAsync("wait up...");

            IMessageObject m = await sendMessage;
            Task.WaitAll(sendMessage);

            await m.ModifyAsync($"pong! {(m.CreatedAt - message.CreatedAt).TotalMilliseconds}ms!");
        }
    }
}
