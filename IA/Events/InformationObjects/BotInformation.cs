using System;

namespace Meru.Events
{
    public class BotInformation
    {
        public string Name;
        public string Identifier = ">";

        public BotInformation()
        {
        }

        public BotInformation(Action<BotInformation> info)
        {
            info.Invoke(this);
        }
    }
}