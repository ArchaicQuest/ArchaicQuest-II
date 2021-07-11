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

        public Skill.Model.Skill Trip()
        {
            return new Skill.Model.Skill()
            {
                Name = "Trip",
                Description =
                    "Trip your opponent for a small amount of damage and a chance to stun them for 1 round.",
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
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim | ValidTargets.TargetNotSelf,
                Damage = new Dice()
                {
                    DiceMaxSize = 4,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

        }

        public Skill.Model.Skill Haggle()
        {
            return new Skill.Model.Skill()
            {
                Name = "Haggle",
                Description =
                    "Haggling is a useful skill to help get lower prices from shops or vendors, at 100% trained you will get 25% off prices",
                ApplyLevelCheck = false,
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
                Type = SkillType.Passive,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 4,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Resting
            };




        }

        public Skill.Model.Skill HeadButt()
        {
            return new Skill.Model.Skill()
            {
                Name = "Headbutt",
                Description =
                    "Headbutt is a useful skill in close combat, the strength of headbutt is improved if the wearer is wearing a helmet.",
                ApplyLevelCheck = false,
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
                    DiceMaxSize = 12,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }
        public Skill.Model.Skill Charge()
        {
            return new Skill.Model.Skill()
            {
                Name = "Charge",
                Description =
                    "",
                ApplyLevelCheck = false,
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
                    DiceMaxSize = 12,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing
            };

        }

        public Skill.Model.Skill FastHealing()
        {
            return new Skill.Model.Skill()
            {
                Name = "Fast Healing",
                Description =
                    "Healing is doubled per tick",
                ApplyLevelCheck = false,
                SavingThrow = new SavingThrow()
                {
                    Reflex = true
                },
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Passive,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetIgnore,
                Damage = new Dice()
                {
                    DiceMaxSize = 12,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing
            };




        }
    }
}
