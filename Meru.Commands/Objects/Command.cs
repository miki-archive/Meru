using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Meru.Common;
using Miki.Database;

namespace Meru.Commands
{
    public class Command : CommandEntity
    {
        public Func<IMessage, Task> ProcessCommand;
		public ushort DefaultPermissionLevel { get; set; } = 1;

        public Command()
        {            
        }

        public Command(CommandEntity cmd) : base(cmd)
        {           
        }

        public Command(Command cmd) : base(cmd)
        {
        }

		public async Task<bool> CanBeUsedAsync(IMessage msg)
		{
			if(await DbClient.Cache.ExistsAsync($"miki:commands:permissions:{msg.Author.Id}"))
			{
				return await DbClient.Cache.GetAsync<bool>($"miki:commands:permissions:{msg.Author.Id}");
			}

			// TODO roles

			if (await DbClient.Cache.ExistsAsync($"miki:commands:permissions:{msg.Channel.Id}"))
			{
				return await DbClient.Cache.GetAsync<bool>($"miki:commands:permissions:{msg.Channel.Id}");
			}

			if (await DbClient.Cache.ExistsAsync($"miki:commands:permissions:{msg.Channel.Guild.Id}"))
			{
				return await DbClient.Cache.GetAsync<bool>($"miki:commands:permissions:{msg.Channel.Guild.Id}");
			}

			return true;
		}
    }
}
