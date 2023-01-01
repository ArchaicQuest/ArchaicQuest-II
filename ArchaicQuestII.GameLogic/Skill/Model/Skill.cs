using System;
using ArchaicQuestII.GameLogic.Skill.Enum;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Skill.Model
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Formula { get; set; }
        public string Damage { get; set; }
        [Obsolete]
        public Effect.Effect Effect { get; set; }
        public List<Affect> SpellAffects { get; set; } = new List<Affect>();
        public CharacterStatus.Status UsableFromStatus { get; set; } = CharacterStatus.Status.Standing;
        public int Rounds { get; set; }
        public SkillCost Cost { get; set; } = new SkillCost();
        public ValidTargets ValidTargets { get; set; }
        public SkillType Type { get; set; }
        public bool StartsCombat { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public bool Deleted { get; set; } = false;

        public Spell.SkillMessage SkillMessage { get; set; } = new Spell.SkillMessage()
        {
            Miss = new Messages(),
            Death = new Messages(),
            Hit = new Messages()
        };
        public SavingThrow SavingThrow { get; set; }
        public bool ApplyLevelCheck { get; set; }


    }
}
