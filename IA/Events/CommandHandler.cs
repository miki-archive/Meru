using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    class CommandHandlerBuilder
    {
        CommandHandler commandHandler;

        public CommandHandlerBuilder(EventSystem eventSystem)
        {
            commandHandler.eventSystem = eventSystem;
        }

        public CommandHandlerBuilder SetOwner(ulong owner)
        {
            commandHandler.IsPrivate = true;
            commandHandler.Owner = owner;
            return this;
        }

        public CommandHandlerBuilder DisposeInSeconds(int seconds)
        {
            commandHandler.ShouldBeDisposed = true;
            commandHandler.timeDisposed = DateTime.Now.AddSeconds(seconds);
            return this;
        }

        public CommandHandlerBuilder SetPrefix(string value)
        {
            commandHandler.Prefix = new PrefixInstance(value, false, false);
            return this;
        }

        public CommandHandler Build()
        {
            return commandHandler;
        }
    }

    class CommandHandler
    {
        public bool IsPrivate = false;
        public bool ShouldBeDisposed = false;

        public ulong Owner = 0;

        public DateTime TimeCreated = DateTime.Now;
        internal DateTime timeDisposed;

        internal EventSystem eventSystem;

        public PrefixInstance Prefix;

        public List<RuntimeModule> Modules = new List<RuntimeModule>();
        public List<RuntimeCommandEvent> Commands = new List<RuntimeCommandEvent>();

        public CommandHandler(EventSystem eventSystem)
        {
            this.eventSystem = eventSystem;
        }

        public async Task CheckAsync(IDiscordMessage msg, )
        {
            if (IsPrivate)
            {
                if (msg.Author.Id == Owner)
                {

                }

                return;
            }

        }

        
    }
}
