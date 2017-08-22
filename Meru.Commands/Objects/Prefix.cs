using System;
using System.Collections.Generic;
using System.Text;

namespace Meru.Commands
{
    public class Prefix
    {
        public string Value { get; set; }

        public Prefix(string defaultValue)
        {
            Value = defaultValue;
        }
    }
}
