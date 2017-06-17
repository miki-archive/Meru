using Meru.Addons;
using Meru.API;
using Meru.Clients;
using Meru.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru
{
    public class Client
    {
        public static Client Instance => _instance;
        protected static Client _instance;

        public AddonManager Addons { protected set; get; }
        public EventSystem Events { protected set; get; }
        public IMessageAPI MessageAPI { get; }

        public string Name => clientInformation.Name;
        public string Version => clientInformation.Version;

        public const string VersionNumber = "1.6";

        protected ClientInformation clientInformation;

        protected virtual async Task Init()
        {
            Events = new EventSystem(x =>
            {
                x.Name = clientInformation.Name;
            });

            Events.RegisterPrefixInstance(">").RegisterAsDefault();
            // fallback prefix
            Events.RegisterPrefixInstance("miki.", false);
            // debug prefix
            Events.RegisterPrefixInstance("fmiki.", false, true);

            Addons = new AddonManager();
            await Addons.Load(this);

            if (clientInformation.EventLoaderMethod != null)
            {
                await clientInformation.EventLoaderMethod(this);
            }
        }
    }
}
