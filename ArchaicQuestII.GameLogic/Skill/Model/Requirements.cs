using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;

namespace ArchaicQuestII.GameLogic.Skill.Model
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
        public CharacterStatus.Status UsableFromStatus { get; set; } = CharacterStatus.Status.Standing;
    }
}
