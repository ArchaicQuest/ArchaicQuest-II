using System;

namespace ArchaicQuestII.GameLogic.Character
{
    public class Player: Character
    {
        public DateTime JoinedDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime LastCommandTime { get; set; }
        public long PlayTime { get; set; } = 0;
        
    }
}
