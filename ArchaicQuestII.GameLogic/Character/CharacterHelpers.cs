using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Character;

public static class CharacterHelpers
{
    public static void AddSkills(this Player player, SubClassName className)
    {
        var c = CoreHandler.Instance.CharacterHandler.GetClass(className); 

        if(c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static void AddSkills(this Player player, ClassName className)
    {
        var c = CoreHandler.Instance.CharacterHandler.GetClass(className); 

        if(c != null)
        {
            player.Skills.AddRange(c.Skills);
        }
    }

    public static SkillList GetSkill(this Player player, SkillName skillName)
    {
        return player.Skills.FirstOrDefault(x =>
            x.Name == skillName && player.Level >= x.Level);
    }

    public static SkillList GetSkill(this Player player, string skillName)
    {
        return player.Skills.FirstOrDefault(x =>
            x.Name.ToString().ToLower() == skillName && player.Level >= x.Level);
    }

    public static int GetWeaponSkill(this Player player, Item.Item weapon)
    {
        var weaponSkill = player.Skills.FirstOrDefault(x =>
            x.Name == weapon.WeaponType);

        return weaponSkill.Proficiency;
    }

    public static IClass GetClass(this Player player)
    {
        return CoreHandler.Instance.CharacterHandler.GetClass(player.ClassName);
    }

    public static bool HasSkill(this Player player, SkillName skillName)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == skillName);

        if(skill != null) return true;

        return false;
    }

    public static bool RollSkill(this Player player, SkillName skillName, int modifier = 0)
    {
        return player.Skills.FirstOrDefault(x => x.Name == skillName).Proficiency > DiceBag.Roll(1, 1, 100, modifier);
    }

    public static void FailedSkill(this Player player, SkillName name, out string message)
    {
        var skill = player.Skills.FirstOrDefault(x => x.Name == name);

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
        $"<p class='improve'>Your knowledge of {skill.Name} increases by {increase}%.</p>";
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

    /// <summary>
    /// Applies bonus affects to player
    /// </summary>
    /// <param name="direction"></param>
    public static void ApplyAffects(this Player player, Affect affect)
    {
        if (affect.Modifier.Strength != 0)
        {
            player.Attributes.Attribute[EffectLocation.Strength] += affect.Modifier.Strength;
        }

        if (affect.Modifier.Dexterity != 0)
        {
            player.Attributes.Attribute[EffectLocation.Dexterity] += affect.Modifier.Dexterity;
        }

        if (affect.Modifier.Constitution != 0)
        {
            player.Attributes.Attribute[EffectLocation.Constitution] += affect.Modifier.Constitution;
        }

        if (affect.Modifier.Intelligence != 0)
        {
            player.Attributes.Attribute[EffectLocation.Intelligence] += affect.Modifier.Intelligence;
        }

        if (affect.Modifier.Wisdom != 0)
        {
            player.Attributes.Attribute[EffectLocation.Wisdom] += affect.Modifier.Wisdom;
        }

        if (affect.Modifier.Charisma != 0)
        {
            player.Attributes.Attribute[EffectLocation.Charisma] += affect.Modifier.Charisma;
        }

        if (affect.Modifier.HitRoll != 0)
        {
            player.Attributes.Attribute[EffectLocation.HitRoll] += affect.Modifier.HitRoll;
        }

        if (affect.Modifier.DamRoll != 0)
        {
            player.Attributes.Attribute[EffectLocation.DamageRoll] += affect.Modifier.DamRoll;
        }

        if (affect.Modifier.Armour != 0)
        {
            player.ArmorRating.Armour += affect.Modifier.Armour;
            player.ArmorRating.Magic += affect.Modifier.Armour;
        }

        if (affect.Affects == DefineSpell.SpellAffect.Blind)
        {
            player.Affects.Blind = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Berserk)
        {
            player.Affects.Berserk = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.NonDetect)
        {
            player.Affects.NonDectect = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Invis)
        {
            player.Affects.Invis = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.DetectInvis)
        {
            player.Affects.DetectInvis = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.DetectHidden)
        {
            player.Affects.DetectHidden = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Poison)
        {
            player.Affects.Poisoned = true;
        }
        if (affect.Affects == DefineSpell.SpellAffect.Haste
        )
        {
            player.Affects.Haste = true;
        }
    }
    
    public static string UpdateAffect(this Player player, Item.Item item, Affect affect)
    {
        var modBenefits = string.Empty;

        if (item.Modifier.Strength != 0)
        {

            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Strength] += item.Modifier.Strength;

            affect.Modifier.Strength = item.Modifier.Strength;
            modBenefits = $"modifies STR by {item.Modifier.Strength} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.Dexterity != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Dexterity] += item.Modifier.Dexterity;

            affect.Modifier.Dexterity = item.Modifier.Dexterity;
            modBenefits = $"modifies DEX by {item.Modifier.Dexterity} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.Constitution != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Constitution] += item.Modifier.Constitution;

            affect.Modifier.Constitution = item.Modifier.Constitution;
            modBenefits = $"modifies CON by {item.Modifier.Constitution} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.Intelligence != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Intelligence] += item.Modifier.Intelligence;
            affect.Modifier.Intelligence = item.Modifier.Intelligence;
            modBenefits = $"modifies INT by {item.Modifier.Intelligence} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.Wisdom != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Wisdom] += item.Modifier.Wisdom;

            affect.Modifier.Wisdom = item.Modifier.Wisdom;
            modBenefits = $"modifies WIS by {item.Modifier.Wisdom} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.Charisma != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.Charisma] += item.Modifier.Charisma;

            affect.Modifier.Charisma = item.Modifier.Charisma;
            modBenefits = $"modifies CHA by {item.Modifier.Charisma} for { affect.Duration} minutes<br />";

        }

        if (item.Modifier.HP != 0)
        {
            player.Attributes.Attribute[EffectLocation.Hitpoints] += item.Modifier.HP;

            if (player.Attributes.Attribute[EffectLocation.Hitpoints] >
                player.MaxAttributes.Attribute[EffectLocation.Hitpoints])
            {
                player.Attributes.Attribute[EffectLocation.Hitpoints] =
                    player.MaxAttributes.Attribute[EffectLocation.Hitpoints];
            }
        }

        if (item.Modifier.Mana != 0)
        {
            player.Attributes.Attribute[EffectLocation.Mana] += item.Modifier.Mana;

            if (player.Attributes.Attribute[EffectLocation.Mana] >
                player.MaxAttributes.Attribute[EffectLocation.Mana])
            {
                player.Attributes.Attribute[EffectLocation.Mana] =
                    player.MaxAttributes.Attribute[EffectLocation.Mana];
            }
        }

        if (item.Modifier.Moves != 0)
        {
            player.Attributes.Attribute[EffectLocation.Moves] += item.Modifier.Moves;

            if (player.Attributes.Attribute[EffectLocation.Moves] >
                player.MaxAttributes.Attribute[EffectLocation.Moves])
            {
                player.Attributes.Attribute[EffectLocation.Moves] =
                    player.MaxAttributes.Attribute[EffectLocation.Moves];
            }
        }

        if (item.Modifier.HitRoll != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.HitRoll] += item.Modifier.HitRoll;
            affect.Modifier.HitRoll = item.Modifier.HitRoll;

            modBenefits = $"modifies Hit Roll by {item.Modifier.HitRoll} for { affect.Duration} minutes<br />";
        }

        if (item.Modifier.DamRoll != 0)
        {
            affect.Duration = 5;
            player.Attributes.Attribute[EffectLocation.DamageRoll] += item.Modifier.DamRoll;

            affect.Modifier.DamRoll = item.Modifier.DamRoll;
            modBenefits = $"modifies Dam Roll by {item.Modifier.DamRoll} for { affect.Duration} minutes<br />";

        }

        // saves / saving spell

        return modBenefits;
    }

    public static void HarmTarget(this Player victim, int damage)
    {
        victim.Attributes.Attribute[EffectLocation.Hitpoints] -= damage;

        if (victim.Attributes.Attribute[EffectLocation.Hitpoints] < 0)
        {
            victim.Attributes.Attribute[EffectLocation.Hitpoints] = 0;
        }
        
        if (victim.Config.Wimpy > 0 && 
            victim.Attributes.Attribute[EffectLocation.Hitpoints] <= victim.Config.Wimpy)
        {
            victim.Buffer.Clear();
            victim.Buffer.Enqueue("flee");
        }
    }

    public static bool IsAlive(this Player victim)
    {
        return victim.Attributes.Attribute[EffectLocation.Hitpoints] > 0;
    }
}