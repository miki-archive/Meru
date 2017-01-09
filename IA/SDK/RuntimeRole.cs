using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace IA.SDK
{
    class RuntimeRole : DiscordRole, IProxy<IRole>
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

        public override System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(role.Color.R, role.Color.G, role.Color.B);
            }
        }

        public override string Name
        {
            get
            {
                return role.Name;
            }
        }

        public IRole ToNativeObject()
        {
            return role;
        }
    }
}
