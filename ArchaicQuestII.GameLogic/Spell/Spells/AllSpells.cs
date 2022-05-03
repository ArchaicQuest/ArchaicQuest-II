using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class AllSpells
    {
        public Skill.Model.Skill MagicMissile()
        {
            var magicMissile = new Skill.Model.Skill()
            {
                Name = "Magic Missile",
                Description =
                    "Fires a magical dart towards your target dealing 1d4+1 damage.",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
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

            return magicMissile;


        }

        public Skill.Model.Skill CauseWounds()
        {
            var magicMissile = new Skill.Model.Skill()
            {
                Name = "Cause light wounds",
                Description =
                    "You channel negative energy that deals 1d8 + 1 per caster level",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
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

            return magicMissile;


        }

        public Skill.Model.Skill CureWounds()
        {
            var magicMissile = new Skill.Model.Skill()
            {
                Name = "Cure light wounds",
                Description =
                    "You channel positive energy that cures 1d8 + 1 per caster level",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
                    }
                },
                Type = SkillType.Affect,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightSelf,
                Damage = new Dice()
                {
                    DiceMaxSize = 8,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            return magicMissile;


        }

        public Skill.Model.Skill Armour()
        {
            var skill = new Skill.Model.Skill()
            {
                Name = "Armour",
                Description =
                    "Baths the target in a protective white light",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
                    }
                },
                Type = SkillType.Affect,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightSelf,
                Damage = new Dice()
                {
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            return skill;


        }

        public Skill.Model.Skill Bless()
        {
            var skill = new Skill.Model.Skill()
            {
                Name = "Bless",
                Description =
                    "Blesses the target increases dam and hit chance",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
                    }
                },
                Type = SkillType.Affect,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            return skill;


        }

        public Skill.Model.Skill Identify()
        {
            var skill = new Skill.Model.Skill()
            {
                Name = "Identify",
                Description =
                    "Tells you the stats of a particular item that identify is cast upon.",
                ApplyLevelCheck = true,
                SavingThrow = new SavingThrow(),
                Rounds = 1,
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>()
                    {
                        {Cost.Mana, 25}
                    }
                },
                Type = SkillType.Passive,
                StartsCombat = false,
                ValidTargets = ValidTargets.TargetObjectInventory | ValidTargets.TargetObjectEquipped,
                Damage = new Dice()
                {
                },
                UsableFromStatus = CharacterStatus.Status.Standing
            };

            return skill;


        }
    }
}
