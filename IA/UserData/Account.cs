using IA.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.UserData
{
    class Account
    {
        public Achievements achievements;
        public FriendsList friendsList;
        public GamesPlayed gamesPlayed;
        public Profile profile;
        public UserInformation info;

        DateTime lastExperienceGain;
    }
}
