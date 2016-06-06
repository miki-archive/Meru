using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Data
{
    interface ISQLReadable
    {
        void LoadFromSQL(ulong id);
        void SendToSQL(ulong id);
    }
}
