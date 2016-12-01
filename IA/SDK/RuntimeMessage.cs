using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeMessage : DiscordMessage, IDiscordMessage
    {
        IMessage messageData = null;

        RuntimeGuild guild = null;
        RuntimeChannel channel = null;
        RuntimeUser user = null;

        public override ulong Id
        {
            get
            {
                return messageData.Id;
            }
        }

        public override IDiscordUser Author
        {
            get
            {
                return user;
            }
        }

        public override IDiscordUser Bot
        {
            get
            {
                return new RuntimeUser((Guild.GetUserAsync(IA.Bot.instance.Client.CurrentUser.Id).GetAwaiter().GetResult() as IProxy<IUser>).ToNativeObject());
            }
        }

        public override DiscordChannel Channel
        {
            get
            {
                return channel;
            }
        }

        public override DiscordGuild Guild
        {
            get
            {
                return guild;
            }
        }

        public override string Content
        {
            get
            {
                return messageData.Content;
            }
        }

        public override IReadOnlyCollection<ulong> MentionedUserIds
        {
            get
            {
                return messageData.MentionedUserIds;
            }
        }
        public override IReadOnlyCollection<ulong> MentionedRoleIds
        {
            get
            {
                return messageData.MentionedRoleIds;
            }
        }

        public override DateTimeOffset Timestamp
        {
            get
            {
                return messageData.Timestamp;
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

        public override async Task DeleteAsync()
        {
            await messageData.DeleteAsync();
        }

        public override async Task ModifyAsync(string message)
        {
            await (messageData as IUserMessage)?.ModifyAsync(x =>
            {
                x.Content = message;
            });
        }

        public override async Task PinAsync()
        {
            await (messageData as IUserMessage)?.PinAsync();
        }

        public override async Task UnpinAsync()
        {
            await (messageData as IUserMessage)?.UnpinAsync();
        }
    }
}
