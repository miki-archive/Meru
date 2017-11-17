using Discord;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IA.SDK
{
    internal class RuntimeAudioChannel : IDiscordAudioChannel
    {
        private IVoiceChannel audio;

        public RuntimeAudioChannel(IVoiceChannel a)
        {
            audio = a;
        }

        public IDiscordGuild Guild
        {
            get
            {
                return new RuntimeGuild(audio.Guild);
            }
        }

        public ulong Id
        {
            get
            {
                return audio.Id;
            }
        }

        public string Name
        {
            get
            {
                return audio.Name;
            }
        }

        public async Task<IDiscordAudioClient> ConnectAsync()
        {
            return new RuntimeAudioClient(await audio.ConnectAsync());
        }

<<<<<<< HEAD
        Task<List<IDiscordUser>> IDiscordChannel.GetUsersAsync()
=======
        public Task<List<IDiscordUser>> GetUsersAsync()
>>>>>>> 0772d4ae30dee1d83720586ac465d225d96368ef
        {
            throw new NotImplementedException();
        }
    }
}