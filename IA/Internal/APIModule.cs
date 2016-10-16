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
                x.processCommand = (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    SQL.Query("SELECT id FROM ia.sharddata where id=?id", output =>
                    {
                        if (output != null)
                        {
                            SQL.Query("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                                    (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                    bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                    bot.Client.Guilds.Count,
                                    bot.GetShardId()
                                    );
                        }
                        else
                        {
                            SQL.Query("INSERT INTO ia.sharddata (id, users, channels, servers) VALUES (?id, ?users, ?channels, ?servers)", null,
                                      bot.GetShardId(),
                                      (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                      bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                      bot.Client.Guilds.Count
                                      );
                        }
                    }, bot.GetShardId());
                };
            });

            // leave api call
            bot.Events.AddLeaveEvent(x =>
            {
                x.name = "leaveapicall";
                x.canBeDisabled = false;
                x.processCommand = (e) =>
                {
                    SQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    SQL.Query("SELECT id FROM ia.sharddata where id=?id", output =>
                    {
                        if (output != null)
                        {
                            SQL.Query("UPDATE ia.sharddata users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                                    (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                    bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                    bot.Client.Guilds.Count,
                                    bot.GetShardId()
                                    );
                        }
                        else
                        {
                            SQL.Query("INSERT INTO ia.sharddata (id, users, channels, servers) VALUES (?id, ?users, ?channels, ?servers)", null,
                                      bot.GetShardId(),
                                      (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                      bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                      bot.Client.Guilds.Count
                                      );
                        }
                    }, bot.GetShardId());
                };
            }); 
        }
    }
}
