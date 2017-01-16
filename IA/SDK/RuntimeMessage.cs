using Discord;
using Discord.WebSocket;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeMessage : IDiscordMessage
    {
        IMessage messageData = null;

        RuntimeGuild guild = null;
        RuntimeMessageChannel channel = null;
        RuntimeUser user = null;
        RuntimeClient client = null;

        public ulong Id
        {
            get
            {
                return messageData.Id;
            }
        }

        public IDiscordUser Author
        {
            get
            {
                return user;
            }
        }

        public IDiscordUser Bot
        {
            get
            {
                return new RuntimeUser((Guild.GetUserAsync(IA.Bot.instance.Client.GetShard(0).CurrentUser.Id).GetAwaiter().GetResult() as IProxy<IUser>).ToNativeObject());
            }
        }

        public IDiscordChannel Channel
        {
            get
            {
                return channel;
            }
        }

        public IDiscordGuild Guild
        {
            get
            {
                return guild;
            }
        }

        public string Content
        {
            get
            {
                return messageData.Content;
            }
        }

        public IReadOnlyCollection<ulong> MentionedUserIds
        {
            get
            {
                return messageData.MentionedUserIds;
            }
        }
        public IReadOnlyCollection<ulong> MentionedRoleIds
        {
            get
            {
                return messageData.MentionedRoleIds;
            }
        }

        public DateTimeOffset Timestamp
        {
            get
            {
                return messageData.Timestamp;
            }
        }

<<<<<<< HEAD
        public Interfaces.IDiscordClient Discord
        {
            get
            {
                return client;
=======
        public int ShardId
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IDiscordMessageChannel IDiscordMessage.Channel
        {
            get
            {
                throw new NotImplementedException();
>>>>>>> 7ec0f55567826bbda9ef2bcd49a18c04d2ddebac
            }
        }

        public RuntimeMessage(IMessage msg)
        {
            messageData = msg;

            user = new RuntimeUser(msg.Author);
            channel = new RuntimeChannel(msg.Channel);
            IGuild g = (messageData.Author as IGuildUser)?.Guild;
            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
        }
        public RuntimeMessage(IMessage msg, DiscordSocketClient c)
        {
            messageData = msg;

            user = new RuntimeUser(msg.Author);
            channel = new RuntimeMessageChannel(msg.Channel);
            IGuild g = (messageData.Author as IGuildUser)?.Guild;
            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
            client = new RuntimeClient(c);
        }

        public IDiscordEmbed CreateEmbed()
        {
            return new RuntimeEmbedBuilder(new EmbedBuilder());
        }

        public async Task DeleteAsync()
        {
            await messageData.DeleteAsync();
        }

        public async Task ModifyAsync(string message)
        {
            await (messageData as IUserMessage)?.ModifyAsync(x =>
            {
                x.Content = message;
            });
        }

        public async Task PinAsync()
        {
            await (messageData as IUserMessage)?.PinAsync();
        }

        public async Task UnpinAsync()
        {
            await (messageData as IUserMessage)?.UnpinAsync();
        }
    }
}
