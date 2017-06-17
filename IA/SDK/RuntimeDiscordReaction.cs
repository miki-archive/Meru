using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.SDK
{
    class RuntimeDiscordReaction
    {
        IReaction sourceReaction;



        public RuntimeDiscordReaction(IReaction reaction)
        {
            sourceReaction = reaction;
            
        }

        
    }
}
