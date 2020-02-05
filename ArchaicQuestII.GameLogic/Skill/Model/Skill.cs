using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Skill.Model
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dice Damage { get; set; }
        public Effect.Effect Effect { get; set; }
        public Requirements Requirements { get; set; }
        public Messages SkillStart { get; set; }
        public List<Messages> SkillAction { get; set; }
        public Messages SkillEnd { get; set; }
        public Messages SkillFailure { get; set; }
        public LevelBasedMessages LevelBasedMessages { get; set; }
        public int Rounds { get; set; }
        public SkillCost Cost { get; set; } = new SkillCost();
        public SkillType Type { get; set; }

    }
}
