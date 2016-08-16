using Discord;
using DynamicExpresso;
using IA;
using IA.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_TestBot
{
    class Program
    {
        static ulong setChannel = 0;

        static void Main(string[] argv)
        {
            IABot bot = new IABot();

            bot.Events.AddCommandEvent(ev =>
            {
                ev.name = "eval";
                ev.processCommand = (e, args) =>
                {
                    var interpreter = new Interpreter().SetVariable("bot", bot).SetVariable("this", e);
                    var result = interpreter.Eval(args);
                    e.Channel.SendMessage(result.ToString());
                };
            });
            bot.Events.AddCommandEvent(ev =>
            {
                ev.name = "getevent";
                ev.processCommand = (e, args) =>
                {
                    bot.GetEvent(args);
                };
            });
            bot.Events.AddCommandEvent(ev =>
            {
                ev.name = "simulateload";
                ev.processCommand = async (e, args) =>
                {
                    float minutes = 1;
                    float peakCPU = 0;

                    PerformanceCounter cpuCounter = new PerformanceCounter();

                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";

                    if (args != string.Empty)
                    {
                        minutes = float.Parse(args);
                    }

                    DateTime started = DateTime.Now;
                    DateTime canReadCPU = DateTime.Now.AddSeconds(1);
                    await e.Channel.SendMessage($"Starting benchmarking for **{minutes}** minute");

                    int i = 0;

                    while (started.AddMinutes(minutes) > DateTime.Now)
                    {
                        await bot.Events.ListCommands(e);

                        if (DateTime.Now > canReadCPU)
                        {
                            float curCPU = cpuCounter.NextValue();
                            if (curCPU > peakCPU)
                            {
                                peakCPU = curCPU;
                            }
                            canReadCPU = DateTime.Now.AddSeconds(1);
                        }
                        i++;
                    }

                    await e.Channel.SendMessage($"Benchmarking completed.\n\n**Commands Done:** {i.ToString("N0")}\n**Peak CPU Usage: **{Math.Round(peakCPU, 2)}%");
                };
            });
            bot.Events.AddCommandEvent(ev =>
            {
                ev.name = "ping";
                ev.processCommand = async (e, args) =>
                {
                    DateTime now = DateTime.Now;
                    Message m = await e.Channel.SendMessage("Pong. {x}ms");
                    await Task.Delay(100);
                    await e.Channel.GetMessage(m.Id).Edit($"Pong. {(now - m.Timestamp).TotalMilliseconds}ms");
                };
            });
            bot.Events.AddCommandEvent(ev =>
            {
                ev.name = "setchannel";
                ev.processCommand = (e, args) =>
                {
                    setChannel = e.Channel.Id;
                    e.Channel.SendMessage("Set this channel as message log");
                };
            });
            bot.Events.AddMentionEvent(mentionEvent =>
            {
                mentionEvent.name = "cleverbot";
                mentionEvent.checkCommand = (e, args, a) =>
                {
                    return (e.Message.RawText.StartsWith(bot.Client.CurrentUser.Mention) || e.Message.RawText.StartsWith(bot.Client.CurrentUser.NicknameMention));
                };
                mentionEvent.processCommand = (e, args) =>
                {
                    e.Channel.SendMessage("ay yo fuk u man");
                };
            });
            bot.Events.AddJoinEvent(newJoinEvent =>
            {
                newJoinEvent.name = "joinevent";
                newJoinEvent.processCommand = (eventArgs) =>
                {
                    if (setChannel != 0)
                    {
                        eventArgs.Server.GetChannel(setChannel).SendMessage(eventArgs.User.Name + " has joined the server");
                    }
                };
            });
            bot.Events.AddLeaveEvent(newLeaveEvent =>
            {
                newLeaveEvent.name = "leaveevent";
                newLeaveEvent.processCommand = (eventArgs) =>
                {
                    if (setChannel != 0)
                    {
                        eventArgs.Server.GetChannel(setChannel).SendMessage(eventArgs.User.Name + " has left the server");
                    }
                };
            });
            bot.Connect();
            Console.ReadLine();
        }
    }
}