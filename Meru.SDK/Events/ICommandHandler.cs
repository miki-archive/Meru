using IA.SDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK.Events
{
    public interface ICommandHandler
    {
        bool IsPrivate { get; set; }
        bool ShouldBeDisposed { get; set; }

        ulong Owner { get; set; }

        bool ShouldDispose();

        void AddCommand(ICommandEvent cmd);
        void AddModule(IModule module);

        EventAccessibility GetUserAccessibility(IDiscordMessage message);
        ICommandEvent GetCommandEvent(string id);

        void RequestDispose();
    }
}
