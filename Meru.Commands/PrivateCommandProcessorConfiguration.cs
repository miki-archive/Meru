using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class PrivateCommandProcessorConfiguration : CommandProcessorConfiguration
    {
        public TimeSpan ExpirationSpan { get; set; } = new TimeSpan(0);
        public bool Expires { get; set; } = false;
    }
}
