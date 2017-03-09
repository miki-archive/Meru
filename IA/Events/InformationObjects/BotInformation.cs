using System;

namespace IA.Events
{
    public class BotInformation
    {
        public string Name;
        public string Identifier = ">";
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