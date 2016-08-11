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
        static void Main(string[] argv)
        {
            IABot bot = new IABot(botInfo =>
            {
                botInfo.botName = "ia";
                botInfo.botToken = "MTg4Nzg0MDczMjYxNDQ5MjE3.Cokn6Q.p-NzzIypidnm0webbSNiGmgXiO4";
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


            bot.Connect();
        }
    }
}