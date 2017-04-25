using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// TODO: clean this
namespace IA.SDK
{
    public class RuntimeUser : IDiscordUser, IProxy<IUser>
    {
        private IUser user;

        public RuntimeUser()
        {
        }
        public RuntimeUser(IUser author)
        {
            user = author;
        }

        public string AvatarUrl
        {
            get
            {
                return user.GetAvatarUrl();
            }
        }

        public ulong Id
        {
            get
            {
                return user.Id;
            }
        }

        public bool IsBot
        {
            get
            {
                return user.IsBot;
            }
        }

        public string Username
        {
            get
            {
                return user.Username;
            }
        }

        public string Discriminator
        {
            get
            {
                return user.Discriminator;
            }
        }

        public string Mention
        {
            get
            {
                return user.Mention;
            }
        }

        public List<ulong> RoleIds
        {
            get
            {
                return (user as IGuildUser).RoleIds.ToList();
            }
        }

        public IDiscordGuild Guild
        {
            get
            {
                IGuildUser u = user as IGuildUser;

                if (u == null)
                {
                    return null;
                }

                return new RuntimeGuild(u.Guild);
            }
        }

        public IDiscordAudioChannel VoiceChannel
        {
            get
            {
                return new RuntimeAudioChannel((user as IGuildUser).VoiceChannel);
            }
        }

        public string Nickname
        {
            get
            {
                return (user as IGuildUser).Nickname;
            }
        }

        public DateTimeOffset CreatedAt
        {
            get
            {
                return user.CreatedAt;
            }
        }

        public DateTimeOffset? JoinedAt
        {
            get
            {
                return (user as IGuildUser)?.JoinedAt;
            }
        }

        public async Task Kick()
        {
            await (user as IGuildUser).KickAsync();
        }

        public async Task Ban(IDiscordGuild g)
        {
            IGuild x = (g as IProxy<IGuild>).ToNativeObject();
            await x.AddBanAsync(user);
        }

        public async Task SendFile(string path)
        {
            IDMChannel c = await user.CreateDMChannelAsync();
            await c.SendFileAsync(path);
        }

        public async Task<IDiscordMessage> SendMessage(string message)
        {
            IDMChannel c = await user.CreateDMChannelAsync();

            RuntimeMessage m = new RuntimeMessage(await c.SendMessageAsync(message));
            Log.Message("Sent message to " + user.Username);
            return m;
        }

        public async Task<IDiscordMessage> SendMessage(IDiscordEmbed embed)
        {
            IDMChannel c = await user.CreateDMChannelAsync();
            IMessage m = await c.SendMessageAsync("", false, (embed as IProxy<EmbedBuilder>).ToNativeObject());
            Log.Message("Sent message to " + user.Username);
            return new RuntimeMessage(m);
        }

        public bool HasPermissions(IDiscordChannel channel, params DiscordGuildPermission[] permissions)
        {
            foreach (DiscordGuildPermission p in permissions)
            {
                GuildPermission newP = (GuildPermission)Enum.Parse(typeof(DiscordGuildPermission), p.ToString());

                if (!(user as IGuildUser).GuildPermissions.Has(newP))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task AddRoleAsync(IDiscordRole role)
        {
            await (user as IGuildUser).AddRolesAsync(new List<IRole> { (role as IProxy<IRole>).ToNativeObject() });
        }

        public async Task RemoveRoleAsync(IDiscordRole role)
        {
            IRole r = (role as IProxy<IRole>).ToNativeObject();
            IGuildUser u = (user as IGuildUser);

            await u.RemoveRolesAsync(new List<IRole> { r });
        }

        public IUser ToNativeObject()
        {
            return user;
        }

        public async Task AddRolesAsync(List<IDiscordRole> roles)
        {
            List<IRole> roleList = new List<IRole>();

            foreach (IDiscordRole a in roles)
            {
                roleList.Add((a as IProxy<IRole>).ToNativeObject());
            }

            IGuildUser u = (user as IGuildUser);

            await u.AddRolesAsync(roleList);
        }

        public async Task RemoveRolesAsync(List<IDiscordRole> roles)
        {
            List<IRole> roleList = new List<IRole>();

            foreach (IDiscordRole a in roles)
            {
                roleList.Add((a as IProxy<IRole>).ToNativeObject());
            }

            IGuildUser u = (user as IGuildUser);

            await u.RemoveRolesAsync(roleList);
        }

        public async Task SetNickname(string text)
        {
            await (user as IGuildUser).ModifyAsync(x =>
            {
                x.Nickname = text;
            });
        }
    }
}