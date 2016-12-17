using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    class RuntimeRole : DiscordRole
    {
        IRole role;

        public RuntimeRole(IRole role)
        {
            this.role = role;
        }

        public override ulong Id
        {
            get
            {
                return role.Id;
            }
        }

        public override int Position
        {
            get
            {
                return role.Position;
            }
        }

        public override string Mention
        {
            get
            {
                if (role.IsMentionable)
                {
                    return role.Mention;
                }
                return "";
            }
        }
    }
}
