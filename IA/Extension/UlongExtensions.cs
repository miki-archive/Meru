using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meru
{
    public static class UlongExtensions
    {
        public static long ToDbLong(this ulong l)
        {
            unchecked
            {
                return (long)l;
            }
        }
    }
}
