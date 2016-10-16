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
        /// <summary>
        /// Loads API related events.
        /// </summary>
        /// <param name="bot"></param>
        public static void LoadEvents(Bot bot)
        {
            // join api call
            bot.Events.AddJoinEvent(x =>
            {
                x.name = "joinapicall";
                x.canBeDisabled = false;
                x.processCommand = async (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    await SQL.QueryAsync("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null, 
                        (await bot.Client.GetConnectionsAsync()).Count, 
                        bot.Client.Guilds.Sum(z => { return z.Channels.Count; }), 
                        bot.Client.Guilds.Count, 
                        bot.GetShardId()
                        );
                    Log.Done("x");
                };
            });

            // leave api call
            bot.Events.AddLeaveEvent(x =>
            {
                x.name = "leaveapicall";
                x.canBeDisabled = false;
                x.processCommand = async (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    await SQL.QueryAsync("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                        (await bot.Client.GetConnectionsAsync()).Count,
                        bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                        bot.Client.Guilds.Count,
                        bot.GetShardId()
                        );
                    Log.Done("y");
                };
            }); 
        }
    }
}
