using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Utils
{
    class StatusChecker
    {
        static Dictionary<string, List<ulong>> ranks = new Dictionary<string, List<ulong>>();

        public static void AddMember(string status, ulong id)
        {
            if(!ranks.ContainsKey(status))
            {
                ranks.Add(status, new List<ulong>());
            }
            ranks[status].Add(id);
        }

        public static bool HasStatus(string status, ulong id)
        {
            if(!ranks.ContainsKey(status))
            {
                return false;
            }

            for(int i = 0; i < ranks[status].Count; i++)
            {
                if(ranks[status][i] == id)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
