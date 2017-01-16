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
    class RuntimeChannel : IDiscordChannel, IProxy<IChannel>
    {
        public IChannel channel;

        public RuntimeChannel(IChannel c)
        {
            channel = c;
        }

        public IDiscordGuild Guild
        {
            get
            {
                return new RuntimeGuild((channel as IGuildChannel).Guild);
            }
        }

        public ulong Id
        {
            get
            {
                return channel.Id;
            }
        }

        public string Name
        {
            get
            {
                return channel.Name;
            }
        }

        public async  Task<IEnumerable<IDiscordUser>> GetUsersAsync()
        {
            IEnumerable<IUser> users = await channel.GetUsersAsync().Flatten();
            List<RuntimeUser> outputUsers = new List<RuntimeUser>();

            foreach (IUser u in users)
            {
                outputUsers.Add(new RuntimeUser(u));
            }

            return outputUsers;           
        }

        public  async Task SendFileAsync(string path)
        {
            await (channel as IMessageChannel).SendFileAsync(path);
        }

        public  async Task SendFileAsync(MemoryStream stream, string extension)
        {
            await (channel as IMessageChannel)?.SendFileAsync(stream, extension);
        }

        public  async Task<IDiscordMessage> SendMessage(string message)
        {
            RuntimeMessage m = new RuntimeMessage(await (channel as IMessageChannel).SendMessage(message));
            return m;
        }
        public  async Task<IDiscordMessage> SendMessage(IDiscordEmbed embed)
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
