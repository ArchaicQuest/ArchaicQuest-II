using System;

namespace ArchaicQuestII.GameLogic.Character
{
    public class Player: Character
    {
        public DateTime JoinedDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public DateTime LastCommandTime { get; set; }
        public long PlayTime { get; set; } = 0;
        public bool IsTelnet { get; set; }
        public string Build { get; set; }
        public string Skin { get; set; }
        public string Eyes { get; set; }
        public string Face { get; set; }
        public string HairColour { get; set; }
        public string HairLength { get; set; }
        public string HairTexture { get; set; }
        public string FacialHair { get; set; }
    }
}
