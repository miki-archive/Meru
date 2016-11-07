using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    class DiscordRole : IDiscordEntity, IDiscordRole
    {
        public ulong Id
        {
            get
            {
                return 0;
            }
        }

        public int Position
        {
            get
            {
                return 0;
            }
        }
    }
}
