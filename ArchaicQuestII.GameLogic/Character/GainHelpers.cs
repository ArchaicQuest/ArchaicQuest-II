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
    }
}
