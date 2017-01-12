using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    class RuntimeAudioChannel : IDiscordAudioChannel
    {
        IAudioChannel audio;

        public RuntimeAudioChannel(IAudioChannel a)
        {
            audio = a;
        }

        public IDiscordGuild Guild
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ulong Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Task<IDiscordAudioClient> ConnectAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IDiscordUser>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
