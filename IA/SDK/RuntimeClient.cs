using Discord;
using Discord.WebSocket;
using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class RuntimeClient : Interfaces.IDiscordClient
    {
        DiscordSocketClient client;

        public RuntimeClient(DiscordSocketClient c)
        {
            client = c;
        }

        public List<IDiscordGuild> Guilds
        {
            get
            {
                List<IDiscordGuild> g = new List<IDiscordGuild>();
                foreach(IGuild guild in client.Guilds)
                {
                    g.Add(new RuntimeGuild(guild));
                }
                return g;
            }
        }

        public int ShardId
        {
            get
            {
                return client.ShardId;
            }
        }

        public IDiscordUser GetUser(ulong id)
        {
            throw new NotImplementedException();
        }

        public async Task SetGameAsync(string game, string link = "")
        {
            await client.SetGameAsync(game, (string.IsNullOrEmpty(link) ? null : link), (string.IsNullOrEmpty(link) ? Discord.StreamType.NotStreaming : Discord.StreamType.Twitch));
        }
    }
}
