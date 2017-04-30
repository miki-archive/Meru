using Discord;
using Discord.Audio;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeAudioClient : IDiscordAudioClient, IProxy<IAudioClient>
    {
        private IAudioClient client;

        private Queue<IAudio> queue = new Queue<IAudio>();

        public static async Task<RuntimeAudioClient> CreateAsync(RuntimeUser u)
        {
            return new RuntimeAudioClient(await (u.ToNativeObject() as IGuildUser).VoiceChannel?.ConnectAsync());
        }

        public RuntimeAudioClient(IAudioClient client)
        {
            this.client = client;
            client.Disconnected += AudioClient_Disconnected;
        }

        public Queue<IAudio> AudioQueue
        {
            get
            {
                return queue;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return queue.Count > 0;
            }
        }

        public async Task DisconnectAsync()
        {
            await client.StopAsync();
        }

        public async Task PauseAsync()
        {
            await Task.CompletedTask;
            //TODO Add Pause
        }

        public async Task PlayAsync(IAudio audio, bool skipIfPlaying = false)
        {
            await Task.CompletedTask;
            //TODO Add Play
        }

        public async Task PlayFileAsync(string file)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "./ffmpeg.exe",
                Arguments = $"-i {file} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            Process p = Process.Start(ffmpeg);
            
            var output = p.StandardOutput.BaseStream;

            var discord = client.CreateOpusStream(1920);

            await output.CopyToAsync(discord);

            await discord.FlushAsync();
        }


        public async Task SkipAsync()
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

        public async Task ConnectAsync(IDiscordAudioChannel v)
        {
            client = ((await v.ConnectAsync()) as IProxy<IAudioClient>).ToNativeObject();
        }
    }
}