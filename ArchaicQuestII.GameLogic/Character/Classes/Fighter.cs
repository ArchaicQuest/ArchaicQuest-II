using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Effect;

namespace ArchaicQuestII.GameLogic.Character.Class;

public class Fighter : IClass
{
    public string Name => ClassName.Fighter.ToString();
    public string Description => "Warriors are lethal combatants who can use any weapon and armor with ease, " +
                    "relying on their strength and endurance instead of mana. With a wide range " +
                    "of offensive and defensive skills, they are a versatile class suitable for " +
                    "any race. For beginners, we highly recommend choosing a Human Warrior, as " +
                    "their high hit points and straightforward playstyle make them an easy class to learn.";
                    
    public string PreferredWeapon => SkillName.LongBlade.ToString();
    public string HitDice => "1D10";
    public int ExperiencePointsCost => 1000;
    public string CreatedBy => "Malleus";
    public DateTime DateCreated => DateTime.Now;
    public DateTime DateUpdated => DateTime.Now;

    public Attributes AttributeBonus => new Attributes()
    {
        Attribute = new Dictionary<EffectLocation, int>()
        {
            { EffectLocation.Strength, 2 },
        }
    };

    public List<SubClassName> Reclasses => new List<SubClassName> 
    {   SubClassName.Ranger, 
        SubClassName.Barbarian, 
        SubClassName.Swashbuckler, 
        SubClassName.Armsman, 
        SubClassName.Samurai
    };

    public List<SkillList> Skills => new List<SkillList>
    {
        new SkillList
        {
            Name = SkillName.LongBlade,
            Level = 1,
            Proficiency = 75,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.ShortBlade,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Axe,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Flail,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Polearm,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Hammer,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Spear,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Staff,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Whip,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Unarmed,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Crafting,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Cooking,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Foraging,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Lore,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Elbow,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.DirtKick,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Kick,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Crossbow,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Bow,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Parry,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.FastHealing,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.ShieldBlock,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Charge,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Rescue,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.UpperCut,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Trip,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Stab,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Mount,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.SecondAttack,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Disarm,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.EnhancedDamage,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.WarCry,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.ShieldBash,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Lunge,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.BlindFighting,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.DualWield,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.ThirdAttack,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.HamString,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Slash,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Impale,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.Cleave,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.FourthAttack,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.OverheadCrush,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
        new SkillList
        {
            Name = SkillName.FifthAttack,
            Level = 1,
            Proficiency = 0,
            IsSpell = false,
        },
    };
}