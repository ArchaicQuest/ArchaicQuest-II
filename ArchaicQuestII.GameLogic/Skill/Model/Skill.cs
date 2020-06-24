using System;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Status;

namespace ArchaicQuestII.GameLogic.Skill.Model
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dice Damage { get; set; }
        public Effect.Effect Effect { get; set; }
        public CharacterStatus.Status UsableFromStatus { get; set; } = CharacterStatus.Status.Standing;
        public int Rounds { get; set; }
        public SkillCost Cost { get; set; } = new SkillCost();
        public ValidTargets ValidTargets { get; set; }
        public SkillType Type { get; set; }
        public bool StartsCombat { get; set; } = false;

    }
}
