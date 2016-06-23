using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Diagnostics;
using IA.Events;
using System.Threading;

namespace IA
{
    class Program
    {
        public static DiscordClient client;
        public static EventSystem events;

        static void Main(string[] args) => new Program().Start();

        void Start()
        {
            LoadEvents();
            client = new DiscordClient(x =>
            {
                x.AppName = "Miki";
                x.AppVersion = "0.1.6";
            });

            client.Ready += Client_Ready;
            client.MessageReceived += Client_MessageReceived;
            client.JoinedServer += Client_JoinedServer;

            client.ExecuteAndWait(async () =>
            {
                await client.Connect(Global.ApiKey);
            });
        }

        private async void Client_JoinedServer(object sender, ServerEventArgs e)
        {
            await e.Server.DefaultChannel.SendMessage(":notes: **Hello! I am Miki**\n\n::question: use '" + events.GetPrefix(e.Server.Id) + "help' to see my commands!\n:star: Have a nice day!");
        }

        void LoadEvents()
        {
            events = new EventSystem(bot =>
            {
                bot.Name = "Miki";
                bot.SqlInformation = new SQLInformation(sql =>
                {
                    sql.dataSource = "localhost";
                    sql.database = "ia";
                    sql.port = 3306;
                    sql.username = "root";
                    sql.password = "laikaxx1";
                });
            });
            EventSystem.developers.Add(121919449996460033);

            //Help
            events.AddEvent(x =>
            {
                x.name = "help";
                x.processCommand = async (e, args) =>
                {
                    string output = await events.ListCommands(e.Channel.Id);
                    await e.Channel.SendMessage(output);
                };
            });

            //Info
            events.AddEvent(x =>
            {
                x.name = "info";
                x.processCommand = async (e, args) =>
                {
                    await e.Channel.SendMessage(client.Config.AppName + " v" + client.Config.AppVersion + "\n :desktop: Created by `Veld#5128`");
                };
            });

            //Prefix
            events.AddEvent(x =>
            {
                x.name = "prefix";
                x.processCommand = async (e, args) =>
                {
                    if (e.Message.RawText.Split(' ').Length > 1)
                    {
                        events.SetPrefix(e, e.Message.RawText.Split(' ')[1]);
                        await e.Channel.SendMessage("Prefix changed to `" + events.GetPrefix(e.Server.Id) + "`!");
                    }
                };
            });

            //Toggle
            events.AddEvent(x =>
            {
                x.name = "toggle";
                x.processCommand = async (e, args) =>
                {
                    if (args.Length > 1)
                    {
                        events.Toggle(e);
                        await e.Channel.SendMessage(":white_check_mark: toggled `" + args + "`");
                    }
                    else
                    {
                        await e.Channel.SendMessage(":no_entry_sign: please specify which command you want to enable!");
                    }
                };
            });

            //Node
            events.AddEvent(x =>
            {
                x.name = "node";
                x.accessibility = EventAccessibility.DEVELOPERONLY;
                x.processCommand = async (e, args) =>
                {
                    Log.Message("entered 'node'");
                    string id = e.Message.Text.Split(' ')[1];
                    Log.Message("entering Node.Run");
                    string output = await Node.Run(id, args);
                    Log.Message("finished Node.Run");
                    if (output != "")
                    {
                        await e.Channel.SendMessage(output);
                    }
                };
            });

            //NodeRealtime
            events.AddEvent(x =>
            {
                x.name = "noderealtime";
                x.accessibility = EventAccessibility.DEVELOPERONLY;
                x.processCommand = async (e, args) =>
                {
                    Log.Message("entered 'node-realtime'");
                    string id = e.Message.Text.Split(' ')[1];
                    Log.Message("entering Node.Run");
                    new Thread(new Node(id, args, e.Channel).Run).Start();
                    Log.Message("finished Node.Run");
                    await Task.Delay(0);
                };
            });

            //Say
            events.AddEvent(x =>
             {
                 x.name = "say";
                 x.accessibility = EventAccessibility.DEVELOPERONLY;
                 x.deletesMessage = true;
                 x.processCommand = async (e, args) =>
                 {
                     await e.Channel.SendMessage(args);
                 };
             });

            //Ping
            events.AddEvent(x =>
            {
                x.name = "ping";
                x.processCommand = async (e, args) =>
                {
                    DateTime pongTime = DateTime.Now;
                    int ping = (pongTime - e.Message.Timestamp).Milliseconds;
                    Discord.Message m = await e.Channel.SendMessage((ping < 500 ? ":green_heart:" : ping > 900 ? ":heart:" : ":yellow_heart:") + "Pong! " + ping + "ms!");
                };
            });

            //CNode
            events.AddEvent(x =>
            {
                x.name = "cnode";
                x.accessibility = EventAccessibility.DEVELOPERONLY;
                x.processCommand = async (e, args) =>
                {
                    string id = e.Message.Text.Split(' ')[1];
                    string code = e.Message.Text.Substring(7 + id.Length);
                    Node.Create(id, code);
                    await e.Channel.SendMessage(":white_check_mark: node " + id + " created!");
                };
            });

            //Stats
            events.AddEvent(x =>
            {
                x.name = "stats";
                x.accessibility = EventAccessibility.DEVELOPERONLY;
                x.processCommand = async (e, args) =>
                {
                    await e.Channel.SendMessage("Ram: " + Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "mb");
                };
            });

            //Roll
            events.AddEvent(x =>
            {
                x.name = "roll";
                x.processCommand = async (e, args) =>
                {
                    Random r = new Random();
                    string rollCalc = "";
                    string amount = "";
                    int rollAmount = 0;

                    if (e.Message.Text.Length > 1)
                    {

                        amount = e.Message.Text.Split(' ')[1];
                        if (amount.Split('d').Length > 1)
                        {
                            for (int i = 0; i < int.Parse(amount.Split('d')[0]); i++)
                            {
                                int num = r.Next(0, int.Parse(amount.Split('d')[1]));
                                rollAmount += num;
                                rollCalc += num + " + ";
                            }
                            rollCalc = rollCalc.Remove(rollCalc.Length - 2);
                        }
                        else
                        {
                            rollAmount = r.Next(0, int.Parse(amount));
                        }
                    }
                    else
                    {
                        rollAmount = r.Next(0, 100);
                    }

                    await e.Channel.SendMessage(":game_die: You rolled a **" + rollAmount + "**" + (rollCalc != "" ? " (" + rollCalc + ")" : ""));

                };
            });

            //Cleverbot
            events.AddEvent(x =>
            {
                x.name = "cleverbot";
                x.type = EventType.MENTION;
                x.checkCommand = (c, a, e) =>
                {
                    return e.Message.RawText.StartsWith(client.CurrentUser.Mention) || e.Message.RawText.StartsWith(client.CurrentUser.NicknameMention);
                };
                x.processCommand = async (e, args) =>
                {
                    await e.Channel.SendIsTyping();
                    await e.Channel.SendMessage(":speech_balloon: - " + Node.Run("c", e.Message.RawText.Substring(client.CurrentUser.Id.ToString().Length + 4)).Result);
                };
            });
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            await events.Check(e);
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.Done("Connected, user: " + client.CurrentUser.Name);
            client.SetGame("`help | " + client.Config.AppVersion);
        }
    }
}
