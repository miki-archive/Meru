using IA.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA
{
    class Shard : IpcSocket
    {
        public int id;
        public Process shardProcess;

        public Shard(int id)
        {
            Log.Message("Starting shard " + id);
            this.id = id;
            OpenShard().GetAwaiter().GetResult();
        }

        public async Task OpenShard()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = id.ToString();
            info.FileName = Process.GetCurrentProcess().ProcessName;
            info.CreateNoWindow = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            shardProcess = Process.Start(info);
            shardProcess.EnableRaisingEvents = true;
            shardProcess.OutputDataReceived += (s, e) =>
            {
                Log.Message("[Shard " + id + "] " + e.Data);
            };
            shardProcess.BeginOutputReadLine();
            await Task.Delay(5000);
        }
    }
}
