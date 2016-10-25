using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IA.SQL;
using Discord.Net.Rest;
using IA.Rest;
using IA.Events;

namespace IA.Modules
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
                    MySQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    MySQL.Query("SELECT id FROM ia.sharddata where id=?id", output =>
                    {
                        if (output != null)
                        {
                            MySQL.Query("UPDATE ia.sharddata SET users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                                    (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                    bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                    bot.Client.Guilds.Count,
                                    bot.GetShardId()
                                    );
                        }
                        else
                        {
                            MySQL.Query("INSERT INTO ia.sharddata (id, users, channels, servers) VALUES (?id, ?users, ?channels, ?servers)", null,
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
                    MySQL.TryCreateTable("ia.sharddata (id int unsigned, users int unsigned, channels int unsigned, servers int unsigned)");

                    MySQL.Query("SELECT id FROM ia.sharddata where id=?id", output =>
                    {
                        if (output != null)
                        {
                            MySQL.Query("UPDATE ia.sharddata SET users=?users, channels=?channels, servers=?servers WHERE id=?id", null,
                                    (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                    bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                    bot.Client.Guilds.Count,
                                    bot.GetShardId()
                                    );
                        }
                        else
                        {
                            MySQL.Query("INSERT INTO ia.sharddata (id, users, channels, servers) VALUES (?id, ?users, ?channels, ?servers)", null,
                                      bot.GetShardId(),
                                      (bot.Client.GetConnectionsAsync().GetAwaiter().GetResult()).Count,
                                      bot.Client.Guilds.Sum(z => { return z.Channels.Count; }),
                                      bot.Client.Guilds.Count
                                      );
                        }
                    }, bot.GetShardId());
                };
            });

            // carbon api call
            if (bot.clientInformation.CarbonitexKey != "")
            {
                // leave api call
                bot.Events.AddLeaveEvent(x =>
                {
                    x.name = "carbon-leave";
                    x.canBeDisabled = false;
                    x.processCommand = async (e) =>
                    {
                        RestClient r = new RestClient("https://www.carbonitex.net/discord/data/botdata.php");

                        r.AddHeader("key", bot.clientInformation.CarbonitexKey);
                        r.AddHeader("servercount", (bot.Client.Guilds).Count.ToString());
                        r.AddHeader("shardid", bot.GetShardId().ToString());
                        r.AddHeader("shardcount", bot.clientInformation.ShardCount.ToString());
                        r.AsJson();

                        await r.PostAsync();
                    };
                });

                // leave api call
                bot.Events.AddJoinEvent(x =>
                {
                    x.name = "carbon-join";
                    x.canBeDisabled = false;
                    x.processCommand = async (e) =>
                    {
                        RestClient r = new RestClient("https://www.carbonitex.net/discord/data/botdata.php");

                        r.AddHeader("key", bot.clientInformation.CarbonitexKey);
                        r.AddHeader("servercount", (bot.Client.Guilds).Count.ToString());
                        r.AddHeader("shardid", bot.GetShardId().ToString());
                        r.AddHeader("shardcount", bot.clientInformation.ShardCount.ToString());
                        r.AsJson();

                        await r.PostAsync();
                    };
                });
            }

            // discord.pw
            if (bot.clientInformation.DiscordPwKey != "")
            {
                // leave api call
                bot.Events.AddLeaveEvent(x =>
                {
                    x.name = "discordpw-leave";
                    x.canBeDisabled = false;
                    x.processCommand = async (e) =>
                    {
                        RestClient r = new RestClient("https://bots.discord.pw/api/bots/" + bot.Client.CurrentUser.Id + "/stats");

                        r.SetAuthorisation("", bot.clientInformation.DiscordPwKey);
                        r.AddHeader("server_count", (bot.Client.Guilds).Count.ToString());
                        r.AddHeader("shard_id", bot.GetShardId().ToString());
                        r.AddHeader("shard_count", bot.clientInformation.ShardCount.ToString());

                        await r.PostAsync();
                    };
                });

                // join api call
                bot.Events.AddJoinEvent(x =>
                {
                    x.name = "discordpw-join";
                    x.canBeDisabled = false;
                    x.processCommand = async (e) =>
                    {
                        RestClient r = new RestClient("https://bots.discord.pw/api/bots/" + bot.Client.CurrentUser.Id + "/stats");

                        r.SetAuthorisation("", bot.clientInformation.DiscordPwKey);
                        r.AddHeader("server_count", (bot.Client.Guilds).Count.ToString());
                        r.AddHeader("shard_id", bot.GetShardId().ToString());
                        r.AddHeader("shard_count", bot.clientInformation.ShardCount.ToString());

                        await r.PostAsync();
                    };
                });
            }
        }
    }
}
    