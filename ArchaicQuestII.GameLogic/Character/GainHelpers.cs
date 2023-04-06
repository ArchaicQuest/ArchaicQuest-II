using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character.Gain
{
    public static class GainHelpers
    {
        public static void GainExperiencePoints(this Player player, Player target, out string message)
        {
            var expWorth = GetExpWorth(target);
            var halfPlayerLevel = Math.Ceiling((double)(player.Level / 2m));
            /*
     
            The following only happens If (player level / 2) is Greater than or equal to mob level  
           If (player level / 2) + 2 is Greater than or equal to mob level then Exp Worth is divided by 4
           Else Exp Worth is divided by 2
            
            */
            if (halfPlayerLevel >= target.Level)
            {
                if (halfPlayerLevel + 2 >= target.Level)
                {
                    expWorth /= 4;  
                }
                else
                {
                    expWorth /= 2;
                }
              
            }
  
            player.Experience += expWorth;
            player.ExperienceToNextLevel -= expWorth;

            message = expWorth == 1 ? "<p class='improve'>You gain 1 measly experience point.</p>" :
            $"<p class='improve'>You receive {expWorth} experience points.</p>";
        }

        public static void GainExperiencePoints(this Player player, int amount, out string message)
        {  
            player.Experience += amount;
            player.ExperienceToNextLevel -= amount;

            message = amount == 1 ? "<p class='improve'>You gain 1 measly experience point.</p>" :
            $"<p class='improve'>You receive {amount} experience points.</p>";
        }

        public static void GainLevel(this Player player, out string message)
        {
            player.Level++;
            player.ExperienceToNextLevel = player.Level * 4000; //TODO: have class and race mod

            var hpGain = (player.MaxAttributes.Attribute[EffectLocation.Constitution] / 100m) * 20;
            var minHPGain = (hpGain / 100m) * 20;
            var totalHP = DiceBag.Roll(1, (int)minHPGain, (int)hpGain);

            var manaGain = player.MaxAttributes.Attribute[EffectLocation.Intelligence] / 100m * 20;
            var minManaGain = manaGain / 100m * 20;
            var totalMana = DiceBag.Roll(1, (int)minManaGain, (int)manaGain);

            var moveGain = player.MaxAttributes.Attribute[EffectLocation.Dexterity] / 100m * 20;
            var minMoveGain = manaGain / 100 * 20;
            var totalMove = DiceBag.Roll(1, (int)minMoveGain, (int)moveGain);

            player.MaxAttributes.Attribute[EffectLocation.Hitpoints] += totalHP;
            player.MaxAttributes.Attribute[EffectLocation.Mana] += totalMana;
            player.MaxAttributes.Attribute[EffectLocation.Moves] += totalMove;

            message = $"<p class='improve'>You have advanced to level {player.Level}, you gain: {totalHP} HP, {totalMana} Mana, {totalMove} Moves.</p>";

            SeedData.Classes.SetGenericTitle(player);
        }

        public static int GetExpWorth(this Player character)
        {
            var maxEXP = 10000;
            var exp = character.Level;
            exp += DiceBag.Roll(1, 25, 275);
            exp += character.Equipped.Wielded?.Damage.Maximum ?? 6; // 6 for hand to hand
            exp += character.Attributes.Attribute[EffectLocation.DamageRoll] * 10;
            exp += character.Attributes.Attribute[EffectLocation.HitRoll] + character.Level * 10;
            exp += character.ArmorRating.Armour;

            exp += character.Attributes.Attribute[EffectLocation.Hitpoints] * 3;
            exp += character.Attributes.Attribute[EffectLocation.Mana];
            exp += character.Attributes.Attribute[EffectLocation.Strength];
            exp += character.Attributes.Attribute[EffectLocation.Dexterity];
            exp += character.Attributes.Attribute[EffectLocation.Constitution];
            exp += character.Attributes.Attribute[EffectLocation.Wisdom];
            exp += character.Attributes.Attribute[EffectLocation.Intelligence];
            exp += character.ArmorRating.Magic;
            exp += character.Level * 15;
            //exp += character.Attributes.Attribute[EffectLocation.Moves];
            // boost xp if mob is shielded

            return exp > maxEXP ? maxEXP : exp;
        }

        public static void FailedSkill(this Player player, string name, out string message)
        {
            var skill = player.Skills.FirstOrDefault(x => x.SkillName.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            var increase = DiceBag.Roll(1, 1, 5);

            if (skill == null)
            {
                message = null;
                return;
            }

            if(skill.Proficiency == 100)
            {
                message = null;
                return;
            }

            skill.Proficiency += increase;

            if (skill.Proficiency > 100)
            {
                skill.Proficiency = 100;
            }

            player.GainExperiencePoints(100 * skill.Level / 4, out _);  
            
            message = $"<p class='improve'>You learn from your mistakes and gain {100 * skill.Level / 4} experience points.</p>" +
            $"<p class='improve'>Your knowledge of {skill.SkillName} increases by {increase}%.</p>";
        }

        public static void FailedSpell(this Player player, string name, out string message)
        {
            //remove
            message = null;

            // var spell = player.Spells.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            // var increase = DiceBag.Roll(1, 1, 5);

            // if (spell == null)
            // {
            //     message = null;
            //     return;
            // }

            // if(spell.Proficiency == 100)
            // {
            //     message = null;
            //     return;
            // }

            // spell.Proficiency += increase;

            // if (spell.Proficiency > 100)
            // {
            //     spell.Proficiency = 100;
            // }

            // player.GainExperiencePoints(100 * spell.Level / 4, out _);  
            
            // message = $"<p class='improve'>You learn from your mistakes and gain {100 * spell.Level / 4} experience points.</p>" +
            // $"<p class='improve'>Your knowledge of {spell.Name} increases by {increase}%.</p>";
        }

        public static void FailedPassive(this Player player, string name, out string message)
        {
            //remove
            message = null;

            // var passive = player.Passives.FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            // var increase = DiceBag.Roll(1, 1, 5);

            // if (passive == null)
            // {
            //     message = null;
            //     return;
            // }

            // if(passive.Proficiency == 100)
            // {
            //     message = null;
            //     return;
            // }

            // passive.Proficiency += increase;

            // if (passive.Proficiency > 100)
            // {
            //     passive.Proficiency = 100;
            // }

            // player.GainExperiencePoints(100 * passive.Level / 4, out _);  
            
            // message = $"<p class='improve'>You learn from your mistakes and gain {100 * passive.Level / 4} experience points.</p>" +
            // $"<p class='improve'>Your knowledge of {passive.Name} increases by {increase}%.</p>";
        }
    }
}
