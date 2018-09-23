using System;

namespace ArchaicQuestII.Core.Player
{
    public class Player: Character.Character
    {
        public DateTime JoinedDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime LastCommandTime { get; set; }
        public long PlayTime { get; set; } = 0;
        
    }
}
