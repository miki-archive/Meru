using IA.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Data
{
    public delegate void OnAchievementUpdate(UserInformation info);

    public class AchievementData
    {
        public string icon;
        public string name;

        public bool unlocked;

        public AchievementData()
        {
            icon = ":none:";
            name = "error";
            unlocked = false;
        }
    }
}
