using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Skill
{
    public class DefineSkill
    {
        public Skill.Model.Skill Kick()
        {
            return new Skill.Model.Skill()
            {
                Name = "Kick",
                Description =
                    "Hits the target with a strong kick.",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow()
                {
                    Reflex = true
                },
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Moves, 25}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 8,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }
        public Skill.Model.Skill Elbow()
        {
            return new Skill.Model.Skill()
            {
                Name = "Elbow",
                Description =
                    "Hits the target with a sharp blow with the elbow.",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow()
                {
                    Reflex = true
                },
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Moves, 25}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 4,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }
        public Skill.Model.Skill Lore()
        {
            return new Skill.Model.Skill()
            {
                Name = "Lore",
                Description =
                    "An automatic skill that tells you information about an item when looking or examining.",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow()
                {
                    Reflex = true
                },
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                },
                Type = SkillType.Damage,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetSelfOnly,
                Damage = new Dice()
                {
                    DiceMaxSize = 4,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }
    }
}
