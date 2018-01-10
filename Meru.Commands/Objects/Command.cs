using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Meru.Commands.Objects;
using Meru.Common;
using Meru.Database;

namespace Meru.Commands
{
    public class Command : CommandEntity
    {
        public Func<CommandEventArgs, Task> ProcessCommand;

		private string permissionsKey => $"miki:commands:permissions:{Id}:";

		public ushort DefaultPermissionLevel { get; set; } = 1;

		/// <summary>
		/// If this command is nested in a MultiCommand. Use this field to set it as the default command.
		/// </summary>
		public bool IsDefault { get; set; } = false;

        public Command()
        {            
        }
        public Command(CommandEntity cmd) : base(cmd)
        {          
        }
        public Command(Command cmd) : base(cmd)
        {
			IsDefault = cmd.IsDefault;
        }

		public async Task<bool> CanBeUsedAsync(IMessage msg)
		{
			bool? check = await CheckIdAsync(msg.Author.Id);
			if(check != null)
			{
				return check ?? true;
			}

			check = await CheckIdAsync(msg.Channel.Id);
			if (check != null)
			{
				return check ?? true;
			}

			check = await CheckIdAsync(msg.Channel.Guild.Id);
			if (check != null)
			{
				return check ?? true;
			}

			return true;
		}
		public async Task<bool?> CheckIdAsync(ulong id)
		{
			if (await DbClient.Cache.ExistsAsync(permissionsKey + id))
			{
				if (!await DbClient.Cache.ExistsAsync(permissionsKey + id + ":check"))
				{
					using (var context = DbClient.Create())
					{
						bool allowed = await context.QueryFirstAsync<bool>("select allowed from public.permissions where id = @id", id);
					}
					await DbClient.Cache.AddAsync(permissionsKey + id + ":check", true, new TimeSpan(1, 0, 0));
				}
				return await DbClient.Cache.GetAsync<bool>(permissionsKey + id);
			}
			return null;
		}

		public async Task SetPermissionsForId(ulong id, bool? value)
		{
			if(value == null)
			{
				await DbClient.Cache.RemoveAsync(permissionsKey + id);
				using (var context = DbClient.Create())
				{
					await context.ExecuteAsync(@"DELETE FROM public.permissions WHERE id = @id;", new { id = (long)id });
				}
				return;
			}
			await DbClient.Cache.AddAsync(permissionsKey + id, value);
			using (var context = DbClient.Create())
			{
				await context.ExecuteAsync(@"WITH upsert AS (UPDATE public.permissions SET 
allowed = @value
WHERE 
id= @id
RETURNING *) INSERT INTO public.permissions (id, allowed) 
SELECT @id, @value  WHERE NOT EXISTS (SELECT * FROM upsert);", new { id = (long)id, value });
			}
		}

		public virtual async Task ProcessAsync(CommandEventArgs msg)
		{
			await ProcessCommand(msg);
		}
	}
}
