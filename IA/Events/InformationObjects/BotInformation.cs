using System;

namespace IA.Events
{
    public class BotInformation
    {
        public string Name;
        public PrefixValue Identifier = PrefixValue.Set(">");
        public SqlInformation SqlInformation;

        public BotInformation()
        {
        }

        public BotInformation(Action<BotInformation> info)
        {
            info.Invoke(this);
        }
    }
}