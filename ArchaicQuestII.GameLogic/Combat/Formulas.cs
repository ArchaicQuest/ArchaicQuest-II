using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Combat
{
   public class Formulas: IFormulas
    {
        private readonly IDice _dice;
        public Formulas(IDice dice)
        {
            _dice = dice;
        }
        public int OffensivePoints(Player player)
        {
            var offensePoints = player.Attributes.Attribute[EffectLocation.Strength] / 5
                                + player.Attributes.Attribute[EffectLocation.Dexterity] / 10
                                + player.Attributes.Attribute[EffectLocation.HitRoll];
            // mob always have 100% skills
            var weaponSkill = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? 100 : 1; //weapon types are hardcoded so make hardcoded skills for weapon types

            return offensePoints + offensePoints * weaponSkill / 100;
        }

        public int DefensivePoints(Player player)
        {
            return player.Attributes.Attribute[EffectLocation.Dexterity] / 5 + player.Attributes.Attribute[EffectLocation.Strength] / 10;
        }

        public int BlockPoints(Player player)
        {
            var offensePoints = player.Attributes.Attribute[EffectLocation.Strength] / 5
                                + player.Attributes.Attribute[EffectLocation.Dexterity] / 10;

            var blockSkill = 100; //weapon types are hardcoded so make hardcoded skills for weapon types

            return offensePoints + offensePoints * blockSkill / 100;
        }

        public int ToBlockChance(Player player, Player target)
        {
            return BlockPoints(player) * 100 / (BlockPoints(player) + DefensivePoints(target));
        }

        public int ToHitChance(Player player, Player target)
        {

            //var x = OffensivePoints(player);
            //var y = DefensivePoints(target);
            //var z = OffensivePoints(player) * 100 / (OffensivePoints(player) + DefensivePoints(target));
            return OffensivePoints(player) * 100 / (OffensivePoints(player) + DefensivePoints(target));
        }

        public int DamageReduction(Player defender, int damage)
        {
            var ArRating = defender.ArmorRating.Armour + 1;
            var armourReduction = ArRating / (double)damage;

            if (armourReduction > 4)
            {
                armourReduction = 4;
            }
            
            if (armourReduction <= 0)
            {
                armourReduction = 1;
            }

            return (int)armourReduction;
        }

        public int CalculateDamage(Player player, Player target, Item.Item weapon)
        {
            
            var damage = 0;

            if (weapon != null)
            {
                damage = _dice.Roll(1, weapon.Damage.Minimum, weapon.Damage.Maximum);

            }
            else
            {
                //Hand to hand
                damage = _dice.Roll(1, 1, 6);
            }

            // Enhanced Damage Skill check
            // increase damage is player has enhanced damage skill
            if (false)
            {
            }

            // calculate damage reduction based on target armour
            var armourReduction = DamageReduction(target, damage);

            //refactor this shit
            var strengthMod = 0.5 + player.Attributes.Attribute[EffectLocation.Strength] / (double)100;
            var levelDif = player.Level - target.Level <= 0 ? 1 : player.Level - target.Level;
            var levelMod = levelDif / 2 <= 0 ? 1 : levelDif / 2;
            var enduranceMod = player.Attributes.Attribute[EffectLocation.Moves] / (double)player.MaxAttributes.Attribute[EffectLocation.Moves];
            var criticalHit = 1;

            if (target.Status == CharacterStatus.Status.Sleeping || target.Status == CharacterStatus.Status.Stunned || target.Status == CharacterStatus.Status.Resting)
            {
                criticalHit = 2;
            }


            int totalDamage = (int)(damage * strengthMod * criticalHit * levelMod);

            if (armourReduction > 0)
            {
                totalDamage = totalDamage / armourReduction;
            }

            if (totalDamage <= 0)
            {
                totalDamage = 1;
            }


            return totalDamage;
        }

        public string TargetHealth(Player player, Player target)
        {
            var percentageOfHP = (100 * target.Attributes.Attribute[EffectLocation.Hitpoints]) /
                                 target.MaxAttributes.Attribute[EffectLocation.Hitpoints];

            if (percentageOfHP >= 50)
            {
                if (percentageOfHP >= 100)
                {
                    return "is in excellent condition.";
                }
                if (percentageOfHP >= 95)
                {
                    return "has a few scratches.";
                }
                if (percentageOfHP >= 85)
                {
                    return "has some small wounds and bruises.";
                }
                if (percentageOfHP >= 75)
                {
                    return "has some minor wounds.";
                }
                if (percentageOfHP >= 63)
                {
                    return "has quite a few wounds.";
                }

                if (percentageOfHP >= 50)
                {
                    return "has some big nasty wounds and scratches.";
                }
            }

            if (percentageOfHP >= 40)
            {
                return "looks pretty hurt.";
            }
            if (percentageOfHP >= 30)
            {
                return "has some large wounds.";
            }
            if (percentageOfHP >= 20)
            {
                return "is in bad condition.";
            }
            if (percentageOfHP >= 10)
            {
                return "is nearly dead.";
            }

            return percentageOfHP >= 0 ? "is in awful condition." : "is bleeding awfully from big wounds.";

        }

        public bool IsCriticalHit()
        {
            return _dice.Roll(1, 1, 20) == 20;
        }

        public bool DoesHit(int chance)
        {
            var roll = _dice.Roll(1, 1, 100);
            
            
            return roll switch
            {
                1 => false,
                100 => true,
                _ => chance > roll
            };
        }
    }
}
