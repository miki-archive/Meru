using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    class PrivateCommandProcessor : CommandProcessor
    {
        private PrivateCommandProcessorConfiguration config;

        public PrivateCommandProcessor(object ownerId, PrivateCommandProcessorConfiguration config) : base(config)
        {
            this.config = config;
        }


    }
}
