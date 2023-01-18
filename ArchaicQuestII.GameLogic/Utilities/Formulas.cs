using System;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Enum;

namespace ArchaicQuestII.GameLogic.Utilities
{
    public static class Formulas
    {
        public static int OffensivePoints(Player player, bool useDualWield = false)
        {

            var strengthMod = Math.Round((double)(player.Attributes.Attribute[EffectLocation.Strength] - 20) / 2, MidpointRounding.ToEven);
            var offensePoints = strengthMod + player.Attributes.Attribute[EffectLocation.Dexterity] / 2f;
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

        public static int DefensivePoints(Player player, Player target)
        {

            var defensivePoints = player.ArmorRating.Armour / 4 + 1;


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

            defensivePoints += weaponSkill + player.Attributes.Attribute[EffectLocation.Dexterity] / 2 + levelDif;

            return defensivePoints;
        }

        public static int BlockPoints(Player player)
        {
            var offensePoints = OffensivePoints(player);

            var blockSkill = 1; //weapon types are hardcoded so make hardcoded skills for weapon types

            return offensePoints + blockSkill * 100;
        }

        public static int ToBlockChance(Player player, Player target)
        {
            return BlockPoints(player) - DefensivePoints(target, player);
        }

        public static int ToHitChance(Player player, Player target, bool useDualWield)
        {

            var baseChance = 45;
            var levelDif = player.Level - target.Level <= 0 ? 0 : player.Level - target.Level;

            var total = OffensivePoints(player, useDualWield) - DefensivePoints(target, player) + baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;

            var off = OffensivePoints(player, useDualWield);
            var def = DefensivePoints(target, player);
            var theRest = baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;
            return OffensivePoints(player, useDualWield) - DefensivePoints(target, player) + baseChance + player.Attributes.Attribute[EffectLocation.HitRoll] + levelDif;
        }

        public static int DamageReduction(Player defender, int damage)
        {
            var arRating = defender.ArmorRating.Armour + 1;

            if (defender.ConnectionId == "mob")
            {
                arRating += defender.Attributes.Attribute[EffectLocation.Dexterity] / 2;
            }

            var x = damage + arRating + defender.Attributes.Attribute[EffectLocation.Dexterity] / 4;
            var y = (double)(damage / (double)(damage + arRating + defender.Attributes.Attribute[EffectLocation.Dexterity] / 4));

            var damageAfterReduction = damage * (double)(damage / (double)(damage + arRating + defender.Attributes.Attribute[EffectLocation.Dexterity] / 4));

            return (int)Math.Ceiling((decimal)damageAfterReduction);
        }

        public static int CalculateDamage(Player player, Player target, Item.Item weapon)
        {
            var damage = 0;

            if (weapon != null)
            {

                var skill = player.Skills.FirstOrDefault(x =>
                    x.SkillName.Replace(" ", string.Empty)
                        .Equals(Enum.GetName(typeof(Item.Item.WeaponTypes), weapon.WeaponType)));

                damage = DiceBag.Roll(1, weapon.Damage.Minimum, weapon.Damage.Maximum);

                if (skill != null)
                {

                    damage = (int)(damage * (skill.Proficiency + 1) / 100) + DiceBag.Roll(1, 1, 3); // 1-3 to stop hand to hand being OP earlier levels if weapon dam is less than 1d6
                }
                else
                {
                    damage /= 2;
                }

            }
            else
            {
                //Hand to hand
                damage = DiceBag.Roll(1, 1, 6);
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
            var damageAfterArmourReduction = DamageReduction(target, (int)totalDamage);


            return damageAfterArmourReduction;
        }

        public static string TargetHealth(Player player, Player target)
        {
            var percentageOfHP = 100 * target.Attributes.Attribute[EffectLocation.Hitpoints] /
                                 target.MaxAttributes.Attribute[EffectLocation.Hitpoints];

            if (percentageOfHP >= 50)
            {
                switch (percentageOfHP)
                {
                    case >= 100:
                        return "is in excellent condition";
                    case >= 95:
                        return "has a few scratches";
                    case >= 85:
                        return "has some small wounds and bruises";
                    case >= 75:
                        return "has some minor wounds";
                    case >= 63:
                        return "has quite a few wounds";
                    case >= 50:
                        return "has some big nasty wounds and scratches";
                }
            }

            return percentageOfHP switch
            {
                >= 40 => "looks pretty hurt",
                >= 30 => "has some large wounds",
                >= 20 => "is in bad condition",
                >= 10 => "is nearly dead",
                _ => percentageOfHP >= 0 ? "is in awful condition" : "is bleeding awfully from big wounds"
            };
        }

        public static bool IsCriticalHit()
        {
            return DiceBag.Roll(1, 1, 20) == 20;
        }

        public static bool DoesHit(int chance)
        {
            var roll = DiceBag.Roll(1, 1, 100);
            
            return roll switch
            {
                1 => false,
                100 => true,
                _ => chance > roll
            };
        }
        
        public static float CalculateWeight(Player player)
        {
            var weight = player.Inventory.Sum(item => item.Weight == 0 ? 1 : item.Weight);

            player.Weight = weight;

            return weight;
        }
        
        public static bool SpellSuccess(Player origin, Player target, Skill.Model.Skill spell)
        {
            var spellSkill = origin.Skills.FirstOrDefault(x => x.SkillId.Equals(spell.Id));

            if (spellSkill == null)
            {
                // TODO: log error, we should never get here.
                return false;
            }

            var spellProficiency = spellSkill.Proficiency;
            var success = DiceBag.Roll(1, 1, 101);

            return !(spellProficiency < success);
        }
        
        public static bool SpellAffectsCharacter(Skill.Model.Skill spell)
        {

            return (spell.ValidTargets & ValidTargets.TargetPlayerWorld) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightVictim) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetSelfOnly) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetPlayerRoom) != 0 ||
                   (spell.ValidTargets & ValidTargets.TargetFightSelf) != 0;
        }
        
        public static KeyValuePair<string, string> DamageText(int damage)
        {
            switch (damage)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>scratch</span>", "<span style='color:#2ecc71'>scratches</span>");
                case 5:
                case 6:
                case 7:
                case 8:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>graze</span>", "<span style='color:#2ecc71'>grazes</span>");
                case 9:
                case 10:
                case 11:
                case 12:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>hit</span>", "<span style='color:#2ecc71'>hits</span>");
                case 13:
                case 14:
                case 15:
                case 16:
                    return new KeyValuePair<string, string>("<span style='color:#2ecc71'>injure</span>", "<span style='color:#2ecc71'>injures</span>");
                case 17:
                case 18:
                case 19:
                case 20:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>wound</span>", "<span style='color:yellow'>wounds</span>");
                case 21:
                case 22:
                case 23:
                case 24:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>maul</span>", "<span style='color:yellow'>mauls</span>");
                case 25:
                case 26:
                case 27:
                case 28:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>decimate</span>", "<span style='color:yellow'>decimates</span>");
                case 29:
                case 30:
                case 31:
                case 32:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>devastate</span>", "<span style='color:yellow'>devastates</span>");
                case 33:
                case 34:
                case 35:
                case 36:
                    return new KeyValuePair<string, string>("<span style='color:yellow'>maim</span>", "<span style='color:yellow'>maims</span>");
                case 37:
                case 38:
                case 39:
                case 40:
                    return new KeyValuePair<string, string>("<span style='color:red'>MUTILATE</span>", "<span style='color:red'>MUTILATES</span>");
                case 41:
                case 42:
                case 43:
                case 44:
                    return new KeyValuePair<string, string>("<span style='color:red'>DISEMBOWEL</span>", "<span style='color:red'>DISEMBOWELS</span>");
                case 45:
                case 46:
                case 47:
                case 48:
                    return new KeyValuePair<string, string>("<span style='color:red'>MASSACRE</span>", "<span style='color:red'>MASSACRES</span>");
                case 49:
                case 50:
                case 51:
                case 52:
                    return new KeyValuePair<string, string>("*** <span style='color:red'>D</span>E<span style='color:red'>M</span>O<span style='color:red'>L</span>I<span style='color:red'>S</span>H ***", "*** <span style='color:red'>D</span>E<span style='color:red'>M</span>O<span style='color:red'>L</span>I<span style='color:red'>S</span>H ***");
                default:
                    return new KeyValuePair<string, string>("*** <span style='color:red'>A</span>N<span style='color:red'>N</span>I<span style='color:red'>H</span>I<span style='color:red'>L</span>A<span style='color:red'>T</span>E<span style='color:red'>S</span> ***", "*** <span style='color:red'>A</span>N<span style='color:red'>N</span>I<span style='color:red'>H</span>I<span style='color:red'>L</span>A<span style='color:red'>T</span>E<span style='color:red'>S</span> ***"); ;
            }

        }
        
        public static bool CheckStatusToCast(Player player, out string message)
        {
            message = player.Status switch
            {
                CharacterStatus.Status.Sleeping => "You can't do this while asleep.",
                CharacterStatus.Status.Stunned => "You are stunned.",
                CharacterStatus.Status.Dead => "You can't do this while dead.",
                CharacterStatus.Status.Ghost => "You can't do this while dead.",
                CharacterStatus.Status.Incapacitated => "You can't do this while dead.",
                CharacterStatus.Status.Resting => "You need to stand up before you do that.",
                CharacterStatus.Status.Sitting => "You need to stand up before you do that.",
                CharacterStatus.Status.Busy => "You can't do that right now.",
                _ => string.Empty
            };

            return message == string.Empty;
        }
        
        public static int GainAmount(int value, Player player)
        {
            return player.Status switch
            {
                CharacterStatus.Status.Sleeping => value *= 3,
                CharacterStatus.Status.Resting => value *= 2,
                _ => value
            };
        }
    }
}
