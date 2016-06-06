using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Data
{
    public class Achievement
    {
        AchievementData data;

        public Achievement(Action<AchievementData> action)
        {
            data = new AchievementData();
            action.Invoke(data);
        }
    }
}
