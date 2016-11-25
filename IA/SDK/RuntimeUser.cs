using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using IA.SDK.Interfaces;

namespace IA.SDK
{
    public class RuntimeUser : DiscordUser, IProxy<IUser>
    {
        private IUser user;

        public RuntimeUser(IUser author)
        {
            user = author;
        }

        public RuntimeUser()
        {
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

        public override bool HasPermissions(DiscordChannel channel, params DiscordGuildPermission[] permissions)
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

        public IUser ToNativeObject()
        {
            return user;
        }
    }
}
