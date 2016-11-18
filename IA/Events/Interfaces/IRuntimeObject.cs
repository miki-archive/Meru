using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    interface IToggleable
    {
        Dictionary<ulong, bool> Enabled { get; }

        bool CanBeDisabled { get; }
        bool DefaultEnabled { get; }
    }
}
