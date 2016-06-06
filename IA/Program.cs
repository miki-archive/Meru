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

namespace IA
{
    class Program
    {
        public static EventListener events;
        public static DiscordClient client;

        #region Forms

        public static IA_Userpanel userPanel;

        #endregion

        static void Main(string[] args) => new Program().Start();


        void Start()
        {
            userPanel = new IA_Userpanel();
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
            e.Server.DefaultChannel.SendMessage("Hello! I am IA :notes:\nuse '" + events.Identifier + "help' to see my commands!");
        }

        void Load()
        {
            events = new EventListener();

            events.AddCommandEvent(x =>
            {
                x.name = "help";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(events.List());
                };
            });

            events.AddCommandEvent(x =>
            {
                x.name = "info";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(client.Config.AppName + " v" + client.Config.AppVersion + "\n :desktop: Created by `Veld#5128`");
                };
            });

            events.AddCommandEvent(x =>
            {
                x.name = "prefix";
                x.processCommand = async e =>
                {
                    events.Identifier = e.Message.RawText.Split(' ')[1];
                    await e.Channel.SendMessage("Prefix changed to `" + events.Identifier + "`!");
                };
            });

            events.AddCommandEvent(x =>
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

            events.AddCommandEvent(x =>
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

            events.AddCommandEvent(x =>
            {
                x.name = "say";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage(e.Message.Text.Substring(5));
                    await e.Message.Delete();
                };
            });

            events.AddCommandEvent(x =>
            {
                x.name = "sql";
                x.developerOnly = true;
                x.processCommand = async e =>
                {
                    await SQL.Query(e.Message.RawText.Substring(5), e.Channel);
                };
            });

            events.AddCommandEvent(x =>
            {
                x.name = "ping";
                x.processCommand = async e =>
                {
                    DateTime pongTime = DateTime.Now;
                    int ping = (pongTime - e.Message.Timestamp).Milliseconds;
                    Message m = await e.Channel.SendMessage((ping < 500 ? ":green_heart:" : ping > 900 ? ":heart:" : ":yellow_heart:") + "Pong! " + ping + "ms!");
                };
            });

            events.AddCommandEvent(x =>
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

            events.AddCommandEvent(x =>
            {
                x.name = "stats";
                x.processCommand = async e =>
                {
                    await e.Channel.SendMessage("Ram: " + Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024 + "mb");
                };
            });

            events.AddCommandEvent(x =>
            {
                x.name = "roll";
                x.processCommand = async (e) =>
                {
                    string amount = "";
                    int rollAmount = 0;
                    if (e.Message.Text.Length > 1)
                    {
                        amount = e.Message.Text.Split(' ')[1];
                        if (amount.Split('d').Length > 1)
                        {
                            for (int i = 0; i < int.Parse(amount.Split('d')[0]); i++)
                            {
                                rollAmount += new Random().Next(0, int.Parse(amount.Split('d')[1]));
                            }
                        }
                        else
                        {
                            rollAmount = new Random().Next(0, int.Parse(amount));
                        }
                    }
                    else
                    {
                        rollAmount = new Random().Next(0, 100);
                    }

                    await e.Channel.SendMessage(":game_die: You rolled a **" + rollAmount + "**");

                };
            });

            events.AddMentionEvent(x =>
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
        }

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {    
           await events.OnMessageEvent(e);
        }

        private void Client_Ready(object sender, EventArgs e)
        {
            Log.Done("Connected, user: " + client.CurrentUser.Name);
        }
    }
}
