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
using System.Threading.Tasks;

namespace IA
{
    class Manager : IpcSocket
    {
        public Manager()
        {
            SendAsync(new Process(), "X");
        }
    }
}
