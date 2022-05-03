using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.Combat
{
    public class Formulas : IFormulas
    {
        private readonly IDice _dice;
        public Formulas(IDice dice)
        {
            _dice = dice;
        }
        public int OffensivePoints(Player player, bool useDualWield = false)
        {

            var strengthMod = Math.Round((double)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2, MidpointRounding.ToEven);
            var offensePoints = strengthMod + player.Attributes.Attribute[EffectLocation.Dexterity] / 2;
            var maxWeaponDam = 0;
            var weapon = useDualWield ? player.Equipped.Secondary : player.Equipped.Wielded;

            SkillList getWeaponSkill = null;
            if (weapon != null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                // urgh this is ugly
                getWeaponSkill = player.Skills.FirstOrDefault(x =>
                   x.SkillName.Replace(" ", string.Empty)
                       .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));

                maxWeaponDam = player.Equipped.Wielded.Damage.Maximum;
            }

            if (weapon == null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                getWeaponSkill = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Hand To Hand", StringComparison.CurrentCultureIgnoreCase));
            }

            // mob always have 100% skills
            var weaponSkill = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? 75 : getWeaponSkill?.Proficiency ?? 1; //weapon types are hardcoded so make hardcoded skills for weapon types

            return (int)offensePoints + weaponSkill + maxWeaponDam;
        }

        public int DefensivePoints(Player player, Player target)
        {

            var defensivePoints = (player.ArmorRating.Armour / 4) + 1;


            var levelDif = player.Level - target.Level <= 0 ? 0 : player.Level - target.Level;

            var weapon = player.Equipped.Wielded;
            SkillList getWeaponSkill = null;
            if (weapon != null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                // urgh this is ugly
                getWeaponSkill = player.Skills.FirstOrDefault(x =>
                   x.SkillName.Replace(" ", string.Empty)
                       .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));
            }

            if (weapon == null && !player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase))
            {
                getWeaponSkill = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Equals("Hand To Hand", StringComparison.CurrentCultureIgnoreCase));
            }

            // mob always have 100% skills
            var weaponSkill = player.ConnectionId.Equals("mob", StringComparison.CurrentCultureIgnoreCase) ? 75 : getWeaponSkill?.Proficiency ?? 1; //weapon types are hardcoded so make hardcoded skills for weapon types

            defensivePoints += weaponSkill + (player.Attributes.Attribute[EffectLocation.Dexterity] / 2) + levelDif;

            return defensivePoints;
        }

        public int BlockPoints(Player player)
        {
            var offensePoints = OffensivePoints(player);

            var blockSkill = 1; //weapon types are hardcoded so make hardcoded skills for weapon types

            return offensePoints + (blockSkill * 100);
        }

        public int ToBlockChance(Player player, Player target)
        {
            return BlockPoints(player) - DefensivePoints(target, player);
        }

        public int ToHitChance(Player player, Player target, bool useDualWield)
        {

            var baseChance = 45;
            var levelDif = player.Level - target.Level <= 0 ? 0 : player.Level - target.Level;

            var total = (OffensivePoints(player, useDualWield) - DefensivePoints(target, player)) + baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;

            var off = OffensivePoints(player, useDualWield);
            var def = DefensivePoints(target, player);
            var theRest = baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;
            return (OffensivePoints(player, useDualWield) - DefensivePoints(target, player)) + baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;
        }

        public int DamageReduction(Player defender, int damage)
        {
            var ArRating = defender.ArmorRating.Armour + 1;

            if (defender.ConnectionId == "mob")
            {
                ArRating += defender.Attributes.Attribute[EffectLocation.Dexterity] / 2;
            }

            var x = (damage + ArRating + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4));
            var y = (double)(damage / (double)(damage + ArRating + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4)));

            var damageAfterReduction = damage * (double)(damage / (double)(damage + ArRating + (defender.Attributes.Attribute[EffectLocation.Dexterity] / 4)));

            return (int)Math.Ceiling((decimal)damageAfterReduction);
        }

        public int CalculateDamage(Player player, Player target, Item.Item weapon)
        {

            var damage = 0;

            if (weapon != null)
            {

                var skill = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Replace(" ", string.Empty)
                        .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));

                damage = _dice.Roll(1, weapon.Damage.Minimum, weapon.Damage.Maximum);

                if (skill != null)
                {

                    damage = (int)(damage * (skill.Proficiency + 1) / 100) + _dice.Roll(1, 1, 3); // 1-3 to stop hand to hand being OP earlier levels if weapon dam is less than 1d6
                }
                else
                {
                    damage /= 2;
                }

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


            var strengthMod = Math.Round((double)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2, MidpointRounding.ToEven);
            var levelDif = player.Level - target.Level <= 0 ? 0 : player.Level - target.Level;

            var totalDamage = damage + strengthMod + levelDif + player.Attributes.Attribute[EffectLocation.DamageRoll];

            var criticalHit = 1;

            if (target.Status == CharacterStatus.Status.Sleeping || target.Status == CharacterStatus.Status.Stunned || target.Status == CharacterStatus.Status.Resting)
            {
                criticalHit = 2;
            }

            totalDamage *= criticalHit;

            // calculate damage reduction based on target armour
            var DamageAfterArmourReduction = DamageReduction(target, (int)totalDamage);


            return DamageAfterArmourReduction;
        }

        public string TargetHealth(Player player, Player target)
        {
            var percentageOfHP = (100 * target.Attributes.Attribute[EffectLocation.Hitpoints]) /
                                 target.MaxAttributes.Attribute[EffectLocation.Hitpoints];

            if (percentageOfHP >= 50)
            {
                if (percentageOfHP >= 100)
                {
                    return "is in excellent condition";
                }
                if (percentageOfHP >= 95)
                {
                    return "has a few scratches";
                }
                if (percentageOfHP >= 85)
                {
                    return "has some small wounds and bruises";
                }
                if (percentageOfHP >= 75)
                {
                    return "has some minor wounds";
                }
                if (percentageOfHP >= 63)
                {
                    return "has quite a few wounds";
                }

                if (percentageOfHP >= 50)
                {
                    return "has some big nasty wounds and scratches";
                }
            }

            if (percentageOfHP >= 40)
            {
                return "looks pretty hurt";
            }
            if (percentageOfHP >= 30)
            {
                return "has some large wounds";
            }
            if (percentageOfHP >= 20)
            {
                return "is in bad condition";
            }
            if (percentageOfHP >= 10)
            {
                return "is nearly dead";
            }

            return percentageOfHP >= 0 ? "is in awful condition" : "is bleeding awfully from big wounds";

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
