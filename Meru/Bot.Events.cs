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

        public event Func<IDiscordMessage, IDiscordMessageChannel, Task> MessageDeleted;

        public event Func<IDiscordUser, Task> UserJoin;

        public event Func<IDiscordUser, Task> UserLeft;

        public event Func<IDiscordUser, IDiscordUser, Task> UserUpdated;

        public void LoadEvents()
        {
            Client.UserJoined += async (u) => await UserJoin.Invoke(new RuntimeUser(u));
            Client.UserLeft += async (u) => await UserLeft.Invoke(new RuntimeUser(u));
            Client.UserUpdated += async (u, unew) => await UserUpdated.Invoke(new RuntimeUser(u), new RuntimeUser(unew));

            // TODO: write a ICachable wrapper
            //Client.MessageDeleted += async (m, c) => await MessageDeleted.Invoke(new RuntimeMessage(m));
            Client.MessageReceived += async (m) => await MessageReceived.Invoke(new RuntimeMessage(m));
        }
    }
}