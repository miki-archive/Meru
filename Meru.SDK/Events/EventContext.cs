using IA.SDK.Interfaces;

namespace IA.SDK.Events
{
    public class EventContext
    {
        public string arguments;

        // public IBot bot;
        public ICommandHandler commandHandler;

        public IDiscordMessage message;

        public IDiscordUser Author => message.Author;
        public IDiscordMessageChannel Channel => message.Channel;
        public IDiscordUser CurrentUser => message.Guild.CurrentUser;
        public IDiscordGuild Guild => message.Guild;
    }
}