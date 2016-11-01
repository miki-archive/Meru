using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeMessage : DiscordMessage
    {
        IMessage messageData;

        public override ulong Id
        {
            get
            {
                return messageData.Id;
            }
        }

        public override IDiscordUser Author {
            get
            {
                return new RuntimeUser(messageData.Author);
            }
        }

        public override IDiscordChannel Channel
        {
            get
            {
                return new RuntimeChannel(messageData.Channel);
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

        public IMessage ToIMessage()
        {
            return messageData;
        }
    }
}
