using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Events
{
    public class BotInformation
    {
        public string Name;
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
