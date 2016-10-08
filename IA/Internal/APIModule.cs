using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.Sql;

namespace IA.Internal
{
    public class APIModule
    {
        public static void LoadEvents(Bot bot)
        {
            // join api call
            bot.Events.AddJoinEvent(x =>
            {
                x.name = "joinapicall";
                x.canBeDisabled = false;
                x.processCommand = async (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned");

                    await SQL.QueryAsync("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null, 
                        (await bot.Client.GetConnectionsAsync()).Count, 
                        (await bot.Client.GetGuildsAsync()).Sum(z => { return (z.GetChannelsAsync()).GetAwaiter().GetResult().Count; }), 
                        (await bot.Client.GetGuildsAsync()).Count, 
                        bot.GetShardId()
                        );
                };
            });

            // leave api call
            bot.Events.AddLeaveEvent(x =>
            {
                x.name = "leaveapicall";
                x.canBeDisabled = false;
                x.processCommand = async (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned");

                    await SQL.QueryAsync("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                        (await bot.Client.GetConnectionsAsync()).Count,
                        (await bot.Client.GetGuildsAsync()).Sum(z => { return (z.GetChannelsAsync()).GetAwaiter().GetResult().Count; }),
                        (await bot.Client.GetGuildsAsync()).Count,
                        bot.GetShardId()
                        );
                };
            });
        }
    }
}
