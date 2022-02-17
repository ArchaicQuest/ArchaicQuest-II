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
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim |
                               ValidTargets.TargetNotSelf,
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


        public Skill.Model.Skill Stab()
        {
            return new Skill.Model.Skill()
            {
                Name = "Stab",
                Description =
                    "Stabs the target with a will aimed stab towards vital organs. Does 1d6 damage + weapon damage ",
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
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Uppercut()
        {
            return new Skill.Model.Skill()
            {
                Name = "Uppercut",
                Description =
                    "Uppercut smashes the target with a strong fist to the chin, has a chance to knock off the helmet of an opponents head, if they are not protecting their head the next blow will stun them for 1 round",
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
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill DirtKick()
        {
            return new Skill.Model.Skill()
            {
                Name = "DirtKick",
                Description =
                    "Kicks up a storm of dirt blinding the opponent briefly",
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
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Disarm()
        {
            return new Skill.Model.Skill()
            {
                Name = "Disarm",
                Description =
                    "Disarms the opponent knocking their weapon flying",
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
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Lunge()
        {
            return new Skill.Model.Skill()
            {
                Name = "Lunge",
                Description =
                    "Lunge at your opponent deliver a devastating blow, it's not without risks though",
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
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Berserk()
        {
            return new Skill.Model.Skill()
            {
                Name = "Berserk",
                Description =
                    "Berserk, going into a fit of rage to boost dam numbers",
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
                ValidTargets = ValidTargets.TargetFightSelf | ValidTargets.TargetSelfOnly,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Rescue()
        {
            return new Skill.Model.Skill()
            {
                Name = "Rescue",
                Description =
                    "Rescues target and becomes the new target of the mob fighting the person being rescued",
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
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill SecondAttack()
        {
            return new Skill.Model.Skill()
            {
                Name = "Second Attack",
                Description =
                    "Allows you attack twice a round if successful",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill ThirdAttack()
        {
            return new Skill.Model.Skill()
            {
                Name = "Third Attack",
                Description =
                    "Allows you attack Three times a round if successful",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill FouthAttack()
        {
            return new Skill.Model.Skill()
            {
                Name = "Fouth Attack",
                Description =
                    "Allows you attack four times a round if successful",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill FithAttack()
        {
            return new Skill.Model.Skill()
            {
                Name = "Fith Attack",
                Description =
                    "Allows you attack five times a round if successful",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }

        public Skill.Model.Skill Mount()
        {
            return new Skill.Model.Skill()
            {
                Name = "Mount",
                Description =
                    "Mount a mob and ride around",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };




        }


        public Skill.Model.Skill BlindFighting()
        {
            return new Skill.Model.Skill()
            {
                Name = "Blind Fighting",
                Description =
                    "Allows you to fight while blinded",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Passive,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetSelfOnly,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill ShieldBash()
        {
            return new Skill.Model.Skill()
            {
                Name = "Shield Bash",
                Description =
                    "Bashes opponent with shield",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill DualWield()
        {
            return new Skill.Model.Skill()
            {
                Name = "Dual Wield",
                Description =
                    "The ability to weld two weapons",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Passive,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill EnhancedDamage()
        {
            return new Skill.Model.Skill()
            {
                Name = "Enhanced Damage",
                Description =
                    "Chance to increase damage per hit",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Passive,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill WarCry()
        {
            return new Skill.Model.Skill()
            {
                Name = "War Cry",
                Description =
                    "War cry boosts damage ",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.None,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetSelfOnly,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill Hamstring()
        {
            return new Skill.Model.Skill()
            {
                Name = "Hamstring",
                Description =
                    "Slashes the opponents hamstring affecting their movement.",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill Impale()
        {
            return new Skill.Model.Skill()
            {
                Name = "Impale",
                Description =
                    "Impale the victim painfully with your weapon",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }
        public Skill.Model.Skill Slash()
        {
            return new Skill.Model.Skill()
            {
                Name = "Slash",
                Description =
                    "Slash the victim painfully with your weapon",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill OverheadCrush()
        {
            return new Skill.Model.Skill()
            {
                Name = "Overhead Crush",
                Description =
                    "Crushes the victim with a mighty overhead blow.",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }

        public Skill.Model.Skill Cleave()
        {
            return new Skill.Model.Skill()
            {
                Name = "Cleave",
                Description =
                    "A more powerful form of slash",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }
        public Skill.Model.Skill Grip()
        {
            return new Skill.Model.Skill()
            {
                Name = "Grip",
                Description =
                    "Improves your grip so you're not easily disarmed",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }
        public Skill.Model.Skill Riposte()
        {
            return new Skill.Model.Skill()
            {
                Name = "Riposte",
                Description =
                    "On a successful parry you are able to strike back gaining an extra weaken hit.",
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
                        {Cost.Moves, 0}
                    }
                },
                Type = SkillType.Damage,
                StartsCombat = true,
                ValidTargets = ValidTargets.TargetFightVictim | ValidTargets.TargetPlayerRoom,
                Damage = new Dice()
                {
                    DiceMaxSize = 6,
                    DiceMinSize = 1,
                    DiceRoll = 1
                },
                UsableFromStatus = CharacterStatus.Status.Standing | CharacterStatus.Status.Fighting
            };

            // blind fighting
        }
    }


}