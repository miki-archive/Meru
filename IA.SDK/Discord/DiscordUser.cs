using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class DiscordUser : IDiscordUser, IMentionable
    {
        public virtual ulong Id
        {
            get
            {
                return 0;
            }
        }

        public virtual bool IsBot
        {
            get
            {
                return false;
            }
        }

        public virtual string Username
        {
            get
            {
                return "";
            }
        }
        public virtual string Discriminator
        {
            get
            {
                return "";
            }
        }

        public virtual string Mention
        {
            get
            {
                return "<@!" + Id + ">";
            }
        }

        public virtual bool HasPermissions(DiscordChannel channel, params DiscordChannelPermission[] permissions)
        {
            throw new NotImplementedException();
        }
        public virtual bool HasPermissions(DiscordGuild guild, params DiscordGuildPermission[] permissions)
        {
            throw new NotImplementedException();
        }

        public virtual Task Ban(DiscordGuild guild)
        {
            throw new NotImplementedException();
        }

        public virtual Task Kick()
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendFile(string path)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public virtual async Task<DiscordMessage> SendMessage(string message)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
