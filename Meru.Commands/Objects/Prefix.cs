using Dapper;
using Meru.Common;
using Meru.Database;
using Miki.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru.Commands
{
    public class Prefix
    {
        public string Value { get; set; }
		public bool Configurable { get; set; } = false;
		
        public Prefix(string defaultValue)
        {
            Value = defaultValue;
        }

		public virtual async Task<string> GetPrefixAsync(IMessage msg)
		{
			try
			{
				if (Configurable)
				{
					string prefix = null;

					IGuild guild = msg.Channel.Guild;

					if (await DbClient.Cache.ExistsAsync($"meru:commands:prefix:{msg.Channel.Guild.Id}"))
					{
						prefix = await DbClient.Cache.GetAsync<string>($"meru:commands:prefix:{msg.Channel.Guild.Id}");
						return prefix;
					}

					if (prefix == null)
					{
						using (var context = DbClient.Create())
						{
							prefix = await context.QueryFirstOrDefaultAsync<string>("SELECT value FROM public.prefixes WHERE guild_id = @id", new { id = (long)msg.Channel.Guild.Id });

							await DbClient.Cache.AddAsync($"meru:commands:prefix:{msg.Channel.Guild.Id}", prefix ?? Value);

							if (prefix != null)
							{
								return prefix;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Log.PrintLine(e.Message + e.StackTrace);
			}
			return Value;
		}

		public virtual async Task<bool> MatchesAsync(IMessage msg)
		{
			return msg.Content
				.ToLower()
				.StartsWith(await GetPrefixAsync(msg));
		}

		public async Task SetPrefixAsync(ulong id, string newPrefix)
		{
			if (Configurable)
			{
				using (var context = DbClient.Create())
				{
					await context.ExecuteAsync(@"
WITH upsert AS (UPDATE public.prefixes SET 
value = @prefix
WHERE 
guild_id= @id
RETURNING *) INSERT INTO public.prefixes (guild_id, value) 
SELECT @id, @prefix  WHERE NOT EXISTS (SELECT * FROM upsert);", new { id = (long)id, prefix = newPrefix });
					await DbClient.Cache.AddAsync($"meru:commands:prefix:{id}", newPrefix);
				}
			}
		}
    }
}
