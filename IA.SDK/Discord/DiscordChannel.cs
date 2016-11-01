using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace IA.SDK
{
    public delegate Task SendFileFromPath(string filePath);
    public delegate Task SendFileFromMemoryStream(MemoryStream ms, string message);

    public delegate Task<DiscordMessage> SendStringToDiscord(string message);

    //public delegate Task<IAsyncEnumerable<ulong>> GetUsersFromDiscord();

    public class DiscordChannel : IDiscordChannel
    {
        public virtual ulong Id
        {
            get
            {
                return 0;
            }
        }

        public virtual async Task SendFileAsync(string file)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
        public virtual async Task SendFileAsync(MemoryStream stream, string extension)
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