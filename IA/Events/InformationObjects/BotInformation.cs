using IA.SQL;
using System;

namespace IA.Events
{
    public class BotInformation
    {
        public string Name;
        public PrefixValue Identifier = PrefixValue.Set(">");
        public SQLInformation SqlInformation;

        public BotInformation()
        {
        }

        public BotInformation(Action<BotInformation> info)
        {
            info.Invoke(this);
        }
    }
}