using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IA.SDK
{
    class RuntimeChannel : DiscordChannel, IDiscordChannel, IProxy<IChannel>
    {
        public IChannel channel;

        private RuntimeChannel()
        {

        }
        public RuntimeChannel(IChannel c)
        {
            channel = c;
        }

        public override ulong Id
        {
            get
            {
                return channel.Id;
            }
        }

        public async override Task<IEnumerable<IDiscordUser>> GetUsersAsync()
        {
            IEnumerable<IUser> users = await channel.GetUsersAsync().Flatten();
            List<RuntimeUser> outputUsers = new List<RuntimeUser>();

            foreach (IUser u in users)
            {
                outputUsers.Add(new RuntimeUser(u));
            }

            return outputUsers;           
        }

        public override async Task SendFileAsync(string path)
        {
            await (channel as IMessageChannel).SendFileAsync(path);
        }

        public override async Task SendFileAsync(MemoryStream stream, string extension)
        {
            await (channel as IMessageChannel)?.SendFileAsync(stream, extension);
        }

        public override async Task<DiscordMessage> SendMessage(string message)
        {
            RuntimeMessage m = new RuntimeMessage(await (channel as IMessageChannel).SendMessage(message));
            return m;
        }
        public override async Task<IDiscordMessage> SendMessage(IDiscordEmbedBuilder embed)
        {
            return new RuntimeMessage(
                await (channel as IMessageChannel)
                .SendMessageAsync("", false, (embed as IProxy<EmbedBuilder>)
                .ToNativeObject()));
        }

        public IChannel ToNativeObject()
        {
            return channel;
        }
    }
}
