using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA
{
    public class MeruUtils
    {
        public static async Task<bool> TryAsync(Func<Task> a, Func<Exception, Task> exception = null)
        {
            try
            {
                await a();
            }
            catch(Exception ex)
            {
                await Bot.instance.OnError(ex);
                return false;
            }
            return true;
        }

        public static async Task ReportErrorAsync(Exception ex)
        {
            await Bot.instance.OnError(ex);
        }
    }
}
