using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Character.Status;

namespace ArchaicQuestII.Engine.Skill
{
    public class Requirements
    {
        public int MinLevel { get; set; }
        public bool Good { get; set; }
        public bool Neutral { get; set; }
        public bool Evil { get; set; }
        public bool Male { get; set; }
        public bool Female { get; set; }
        public Attributes MinAttributes { get; set; }
        public Status UsableFromStatus { get; set; } = Status.Standing;
    }
}
