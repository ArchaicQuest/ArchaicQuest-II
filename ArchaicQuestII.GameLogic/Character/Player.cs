using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Config;
using ArchaicQuestII.GameLogic.Character.Model;

namespace ArchaicQuestII.GameLogic.Character
{
    public class Player : Character
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
        public string ReplyTo { get; set; }
        public List<Player> Followers { get; set; } = new List<Player>();
        public string Following { get; set; }
        public bool Grouped { get; set; }
        public new Money Money { get; set; } = new Money() { Gold = 0 };
        public Money Bank { get; set; } = new Money() { Gold = 0 };
        public int Trains { get; set; } = 5;
        public int Practices { get; set; } = 5;
        public int MobKills { get; set; } = 0;
        public int MobDeaths { get; set; } = 0;
        public int PlayerKills { get; set; } = 0;
        public int PlayerDeaths { get; set; } = 0;
        public int QuestPoints { get; set; } = 0;
        public bool Idle { get; set; } = false;
        public bool AFK { get; set; } = false;
        public List<string> CommandLog { get; set; } = new List<string>();
        public string Pose { get; set; }
        public string Title { get; set; }
    }

    public class MobStats
    {
        public int Id { get; set; }
        public int MobKills { get; set; }
        public int PlayerDeaths { get; set; }
        public DateTime Date { get; set; }
    }
}
