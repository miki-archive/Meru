using IA.SDK;
using IA.SDK.Interfaces;
using System;
using System.Threading.Tasks;

namespace IA
{
    public partial class Bot
    {
        public event Func<IDiscordGuild, Task> GuildJoin;

        public event Func<IDiscordGuild, Task> GuildLeave;

        public event Func<IDiscordMessage, Task> MessageReceived;

        //public event Func<IDiscordMessage, IDiscordMessageChannel, Task> MessageDeleted;

        public event Func<IDiscordUser, Task> UserJoin;

        public event Func<IDiscordUser, Task> UserLeft;

        public event Func<IDiscordUser, IDiscordUser, Task> UserUpdated;

        public void LoadEvents()
        {
            Client.UserJoined += async (u) =>
            {
                await MeruUtils.TryAsync(async () =>
                {
                    Task.Run(() => UserJoin.Invoke(new RuntimeUser(u)));
                });
            };
            
            Client.UserLeft += async (u) =>
            {
                await MeruUtils.TryAsync(async () =>
                {
                    Task.Run(() => UserLeft.Invoke(new RuntimeUser(u)));
                });
            };

            Client.UserUpdated += async (u, unew) =>
            {
                await MeruUtils.TryAsync(async () =>
                {
                    RuntimeUser userOld = new RuntimeUser(u);
                    RuntimeUser userNew = new RuntimeUser(unew);
                    Task.Run(() => UserUpdated.Invoke(userOld, userNew));
                });    
            };

            Client.MessageReceived += async (m) =>
            {
                Task.Run(async () =>
                {
                    await MeruUtils.TryAsync(async () =>
                    {
                        RuntimeMessage newMessage = new RuntimeMessage(m);
                        await MessageReceived(newMessage);
                    });
                });
            };
        }
    }
}