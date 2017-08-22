using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    class PrivateCommandProcessorConfiguration : CommandProcessorConfiguration
    {
        public TimeSpan ExpirationSpan = new TimeSpan(0);
        public bool Expires = false;
    }
}
