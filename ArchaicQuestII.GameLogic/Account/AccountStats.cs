using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Account
{
    public class AccountStats
    {
        public int MobKills { get; set; }
        public int PlayerKills { get; set; }
        public int Deaths { get; set; }
        public double TotalPlayTime { get; set; }
        public int ExploredRooms { get; set; }
    }
}
