using System;

namespace ArchaicQuestII.Engine.Character.Model
{
    public class Player: Character
    {
        public DateTime JoinedDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime LastCommandTime { get; set; }
        public long PlayTime { get; set; } = 0;
        
    }
}
