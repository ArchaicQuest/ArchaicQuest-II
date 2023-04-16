using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Effect;

namespace ArchaicQuestII.GameLogic.Character.Class;

public class Cleric : IClass
{
    public int Id { get; set; }
    public bool IsSubClass => false;
    public string Name => ClassName.Cleric.ToString();
    public string Description =>
        "Warriors are lethal combatants who can use any weapon and armor with ease, "
        + "relying on their strength and endurance instead of mana. With a wide range "
        + "of offensive and defensive skills, they are a versatile class suitable for "
        + "any race. For beginners, we highly recommend choosing a Human Warrior, as "
        + "their high hit points and straightforward playstyle make them an easy class to learn.";

    public string PreferredWeapon => SkillName.Hammer.ToString();
    public string HitDice => "1D10";
    public int ExperiencePointsCost => 1000;
    public string CreatedBy => "Malleus";
    public DateTime DateCreated => DateTime.Now;
    public DateTime DateUpdated => DateTime.Now;

    public Attributes AttributeBonus =>
        new Attributes()
        {
            Attribute = new Dictionary<EffectLocation, int>() { { EffectLocation.Strength, 2 }, }
        };

    public List<SubClassName> Reclasses =>
        new List<SubClassName>
        {
            SubClassName.Crusader,
            SubClassName.Druid,
            SubClassName.Shaman,
            SubClassName.Defiler,
            SubClassName.Monk
        };

    public List<Item.Item> StartingGear => new List<Item.Item> { };

    public List<SkillList> Skills =>
        new List<SkillList>
        {
            new SkillList
            {
                Name = SkillName.LongBlades,
                Level = 1,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.ShortBlades,
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
                Proficiency = 25,
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
                Name = SkillName.Fishing,
                Level = 1,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Lore,
                Level = 9,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Dodge,
                Level = 1,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Elbow,
                Level = 2,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.DirtKick,
                Level = 3,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Kick,
                Level = 4,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Crossbow,
                Level = 5,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Bow,
                Level = 5,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Parry,
                Level = 6,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.FastHealing,
                Level = 7,
                Proficiency = 75,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.ShieldBlock,
                Level = 8,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Charge,
                Level = 11,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Rescue,
                Level = 12,
                Proficiency = 25,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.UpperCut,
                Level = 14,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Trip,
                Level = 15,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Stab,
                Level = 17,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Mount,
                Level = 18,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.SecondAttack,
                Level = 18,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Disarm,
                Level = 20,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.EnhancedDamage,
                Level = 22,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.WarCry,
                Level = 25,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.ShieldBash,
                Level = 26,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Lunge,
                Level = 28,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.BlindFighting,
                Level = 29,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.DualWield,
                Level = 31,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.ThirdAttack,
                Level = 32,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.HamString,
                Level = 35,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Slash,
                Level = 36,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Impale,
                Level = 37,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.Cleave,
                Level = 40,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.FourthAttack,
                Level = 42,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.OverheadCrush,
                Level = 46,
                Proficiency = 0,
                IsSpell = false,
            },
            new SkillList
            {
                Name = SkillName.FifthAttack,
                Level = 50,
                Proficiency = 0,
                IsSpell = false,
            },
        };
}
