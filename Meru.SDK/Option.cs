using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.SDK
{
    public class Option
    {
        public string emoji;

        public Func<Task> output; 

        public Option(string emoji, Func<Task> output)
        {
            this.emoji = emoji;
            this.output = output;
        }
    }
}
