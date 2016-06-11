using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Diagnostics;
using IA.Events;
using IA.Data;
using System.Threading;
using IA.Utils;

namespace IA
{
    class Program
    {
        public static DiscordClient client;
        public static CommandListener modules;

        #region Forms

        public static IA_Userpanel userPanel;

        #endregion

        static void Main(string[] args) => new Program().Start();


        void Start()
        {
            userPanel = new IA_Userpanel();
            StatusChecker.AddMember("developer", 121919449996460033);

            new Thread(userPanel.Show).Start();
            Load();
            client = new DiscordClient(x =>
            {
                x.AppName = "IA";
                x.AppVersion = "0.1";
            });

            client.Ready += Client_Ready;
            client.MessageReceived += Client_MessageReceived;
            client.JoinedServer += Client_JoinedServer;

            client.ExecuteAndWait(async () =>
            {
                await client.Connect(Global.ApiKey);
            });
        }

        private void Client_JoinedServer(object sender, ServerEventArgs e)
        {
            e.Server.DefaultChannel.SendMessage(":notes: **Hello! I am IA**\n\n::question: use '" + Global.Identifier + "help' to see my commands!\n:star: Have a nice day!");
        }

        void Load()
        {
            modules = new CommandListener();

            modules.AddCommand("General", x =>
            {
                x.name = "help";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(modules.List(e));
                };
            });

            modules.AddCommand("General",x =>
            {
                x.name = "info";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(client.Config.AppName + " v" + client.Config.AppVersion + "\n :desktop: Created by `Veld#5128`");
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "prefix";
                x.processCommand = async e =>
                {
                   Global.Identifier = e.Message.RawText.Split(' ')[1];
                    await e.Channel.SendMessage("Prefix changed to `" + Global.Identifier + "`!");
                };
            });

            modules.AddCommand("Node-js", x =>
            {
                x.name = "node";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    Log.Message("entered 'node'");
                    string args = "";
                    string id = e.Message.Text.Split(' ')[1];
                    if (e.Message.Text.Split(' ').Length > 2)
                    {
                        args = e.Message.Text.Substring(7 + id.Length);
                    }
                    Log.Message("entering Node.Run");
                    string output = await Node.Run(id, args);
                    Log.Message("finished Node.Run");
                    if (output != "")
                    {
                        await e.Channel.SendMessage(output);
                    }
                };
            });

            modules.AddCommand("Node-js", x =>
            {
                x.name = "node-realtime";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    Log.Message("entered 'node-realtime'");
                    string args = "";
                    string id = e.Message.Text.Split(' ')[1];
                    if (e.Message.Text.Split(' ').Length > 2)
                    {
                        args = e.Message.Text.Substring(7 + id.Length);
                    }
                    Log.Message("entering Node.Run");
                    new Thread(new Node(id, args, e.Channel).Run).Start();
                    Log.Message("finished Node.Run");
                    await Task.Delay(0);
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "say";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(e.Message.Text.Substring(5));
                    await e.Message.Delete();
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "get";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(":desktop:[SQL] " + SQL.GetQuery(e.Message.RawText.Substring(5)));
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "sql";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    await SQL.Query(e.Message.RawText.Substring(5), e.Channel);
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "ping";
                x.processCommand = async e =>
                {
                    DateTime pongTime = DateTime.Now;
                    int ping = (pongTime - e.Message.Timestamp).Milliseconds;
                    Message m = await e.Channel.SendMessage((ping < 500 ? ":green_heart:" : ping > 900 ? ":heart:" : ":yellow_heart:") + "Pong! " + ping + "ms!");
                };
            });

            modules.AddCommand("Node-js", x =>
            {
                x.name = "cnode";
                x.processCommand = async e =>
                {
                    string id = e.Message.Text.Split(' ')[1];
                    string code = e.Message.Text.Substring(7 + id.Length);
                    Node.Create(id, code);
                    await e.Channel.SendMessage(":white_check_mark: node " + id + " created!");
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "stats";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage("Ram: " + Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "mb");
                };
            });

            modules.AddCommand("General", x =>
            {
                x.name = "roll";
                x.processCommand = async (e) =>
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

                    await e.Channel.SendMessage(":game_die: You rolled a **" + rollAmount + "**" + (rollCalc!=""?" (" + rollCalc + ")":""));

                };
            });

            modules.AddMention("General", x =>
            {
                x.name = "cleverbot";
                x.processCommand = async (e) =>
                {
                    if(e.Message.RawText.Trim(new char[] { '!' }).StartsWith("<@" + client.CurrentUser.Id + ">"))
                    {
                        await e.Channel.SendIsTyping();
                        await e.Channel.SendMessage(":speech_balloon: - " + Node.Run("c", e.Message.RawText.Substring(client.CurrentUser.Id.ToString().Length + 4)).Result);
                    }
                };
            });

            modules.FinishLoading();
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            await modules.Check(e);
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.Done("Connected, user: " + client.CurrentUser.Name);
        }
    }
}
