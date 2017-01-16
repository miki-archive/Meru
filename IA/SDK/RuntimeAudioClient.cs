using Discord;
using Discord.Audio;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    class RuntimeAudioClient : DiscordAudioClient, IProxy<IAudioClient>
    {
        IAudioClient client;

        Queue<IAudio> queue = new Queue<IAudio>();

        public static async Task<RuntimeAudioClient> Create(RuntimeUser u)
        {
            return new RuntimeAudioClient(await (u.ToNativeObject() as IGuildUser).VoiceChannel?.ConnectAsync());
        }

        private RuntimeAudioClient(IAudioClient client)
        {
            this.client = client;
            client.Disconnected += AudioClient_Disconnected;
        }

        public override Queue<IAudio> AudioQueue
        {
            get
            {
                return queue;
            }
        }

        public override bool IsPlaying
        {
            get
            {
                return queue.Count > 0;
            }
        }

        public override async Task Disconnect()
        {
            await client.DisconnectAsync();
           
        }

        public override async Task Pause()
        {
            await Task.CompletedTask;
            //TODO Add Pause

        }

        public override async Task Play(IAudio audio, bool skipIfPlaying = false)
        {
            await Task.CompletedTask;
            //TODO Add Play
        }

        public override async Task Skip()
        {
            await Task.CompletedTask;
            //TODO Add Skip
        }

        public IAudioClient ToNativeObject()
        {
            return client;
        }

        private async Task AudioClient_Disconnected(Exception e)
        {
            Log.ErrorAt("AudioClient", e.Message);

            queue.Clear();

            await Task.CompletedTask;
        }
    }
}
