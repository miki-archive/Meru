using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using IA.SDK.Interfaces;

namespace IA.SDK
{
    class RuntimeUser : DiscordUser
    {
        private IUser user;

        public RuntimeUser(IUser author)
        {
            user = author;
        }

        public override ulong Id
        {
            get
            {
                return user.Id;
            }
        }

        public override string Username
        {
            get
            {
                return user.Username;
            }
        }
        public override string Discriminator
        {
            get
            {
                return user.Discriminator;
            }
        }

        public override string Mention
        {
            get
            {
                return "<@!" + Id + ">";
            }
        }

        public override async Task Kick()
        {
            await (user as IGuildUser).KickAsync();
        }

        public override async Task Ban(DiscordGuild g)
        {
            await (g as RuntimeGuild).guild.AddBanAsync(user);
        }

        public override async Task SendFile(string path)
        {
            IDMChannel c = await user.CreateDMChannelAsync();

            await c.SendFileAsync(path);
        }

        public override async Task<DiscordMessage> SendMessage(string message)
        {
            IDMChannel c = await user.CreateDMChannelAsync();

            RuntimeMessage m = new RuntimeMessage(await c.SendMessageAsync(message));
            return m;
        }

        public override bool HasPermissions(DiscordChannel channel, params DiscordChannelPermission[] permissions)
        {
            foreach (DiscordChannelPermission p in permissions)
            {
                if (!(user as IGuildUser).GetPermissions((channel as RuntimeChannel).channel as IGuildChannel).Has((ChannelPermission)Enum.Parse(typeof(ChannelPermission), p.ToString())))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool HasPermissions(DiscordGuild guild, params DiscordGuildPermission[] permissions)
        {
            foreach (DiscordGuildPermission p in permissions)
            {
                if (!(user as IGuildUser).GetPermissions((guild as RuntimeGuild).guild as IGuildChannel).Has((ChannelPermission)Enum.Parse(typeof(ChannelPermission), p.ToString())))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
