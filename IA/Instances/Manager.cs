using IA.Forms;
using IA.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IA
{
    class Manager : IpcSocket
    {
        AppDomain app = AppDomain.CurrentDomain;

        List<Shard> shard = new List<Shard>();

        int shardCount;

        public Manager(int shard_count)
        {
            shardCount = shard_count;
            OpenManager().GetAwaiter().GetResult();

            new Thread(new ThreadStart(StartForm)).Start();
        }

        private async Task Heartbeat()
        {
            for (int i = 0; i < shard.Count; i++)
            {
                if (!shard[i].shardProcess.Responding)
                {
                    Log.Error("[Shard " + i + "] has stopped responding.");
                    shard[i].shardProcess.Kill();
                    shard[i] = new Shard(i);
                }

                if (shard[i].shardProcess.HasExited)
                {
                    Log.Error("[Shard " + i + "] has crashed.");
                    shard[i] = new Shard(i);
                }
            }

            await Task.Delay(1000);
            await Heartbeat();
        }

        private async Task OpenManager()
        {
            app.ProcessExit += App_ProcessExit;

            Process[] p = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            foreach (Process px in p)
            {
                if (px.Id != Process.GetCurrentProcess().Id)
                {
                    px.Kill();
                }
            }

            for (int i = 0; i < shardCount; i++)
            {
                shard.Add(new Shard(i));
            }
            await Task.Run(async () => await Heartbeat());
            await Task.Delay(-1);
        }

        [STAThread]
        private void StartForm()
        {
            Application.EnableVisualStyles();
            Application.Run(new ManagerForm());
        }


        #region events

        private void App_ProcessExit(object sender, EventArgs e)
        {
            foreach (Shard b in shard)
            {
                b.shardProcess.Kill();
            }
        }

        #endregion
    }
}
