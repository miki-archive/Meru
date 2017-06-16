using Discord;
using Discord.WebSocket;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeMessage : IDiscordMessage, IProxy<IMessage>
    {
        private IMessage messageData = null;

        private RuntimeGuild guild = null;
        private RuntimeMessageChannel channel = null;
        private RuntimeUser user = null;
        private RuntimeClient client = null;

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

        public IDiscordMessageChannel Channel
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

        public DateTimeOffset Timestamp => messageData.Timestamp;

        public Interfaces.IDiscordClient Discord => client;

        public int ShardId => client.ShardId;

        // TODO: Implement
        public IDiscordAudioChannel VoiceChannel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<DiscordEmoji, DiscordReactionMetadata> Reactions
        {
            get
            {
                IReadOnlyDictionary<IEmote, ReactionMetadata> x = (messageData as IUserMessage).Reactions;
                Dictionary<DiscordEmoji, DiscordReactionMetadata> emojis = new Dictionary<DiscordEmoji, DiscordReactionMetadata>();
                foreach(Emoji y in x.Keys)
                {
                    DiscordEmoji newEmoji = new DiscordEmoji();
                    newEmoji.Name = y.Name;

                    DiscordReactionMetadata metadata = new DiscordReactionMetadata();
                    metadata.IsMe = x[y].IsMe;
                    metadata.ReactionCount = x[y].ReactionCount;

                    emojis.Add(newEmoji, metadata);
                }
                return emojis;
            }
        }

        public IReadOnlyCollection<ulong> MentionedChannelIds => messageData.MentionedChannelIds;

        public RuntimeMessage(IMessage msg)
        {
            messageData = msg;

            if (msg.Author != null) user = new RuntimeUser(msg.Author);
            if(msg.Channel != null) channel = new RuntimeMessageChannel(msg.Channel);
            IGuild g = (messageData.Author as IGuildUser)?.Guild;
            
            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
        }

        public RuntimeMessage(IMessage msg, DiscordSocketClient c)
        {
            messageData = msg;

            if (msg.Author != null) user = new RuntimeUser(msg.Author);
            if (msg.Channel != null) channel = new RuntimeMessageChannel(msg.Channel);
            IGuild g = (messageData.Author as IGuildUser)?.Guild;
            if (g != null)
            {
                guild = new RuntimeGuild(g);
            }
            client = new RuntimeClient(c);
        }

        public async Task AddReaction(string emoji)
        {
            await (messageData as IUserMessage).AddReactionAsync(new Emoji(emoji));
        }

        // ---------------------------- important :( why don't I ever do what future me would do?!?!!?
        // GET THIS SHIT OUTTA HERE.
        // One day..
        public IDiscordEmbed CreateEmbed()
        {
            return new RuntimeEmbed(new EmbedBuilder());
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
        public async Task ModifyAsync(IDiscordEmbed embed)
        {
            await (messageData as IUserMessage)?.ModifyAsync(x =>
            {
                x.Embed = ((embed as RuntimeEmbed) as IProxy<EmbedBuilder>).ToNativeObject().Build();
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

        public IMessage ToNativeObject()
        {
            return messageData;
        }
    }
}