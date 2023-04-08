using System;
using ArchaicQuestII.GameLogic.Core;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Model;

namespace ArchaicQuestII.GameLogic.Character.Class
{
    public class SkillList
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int Level { get; set; }
        public int? Proficiency { get; set; } = 1;

        public bool IsSpell { get; set; }
    }


    public class Class : OptionDescriptive
    {
        public List<SkillList> Skills { get; set; } = new List<SkillList>();
        public string HitDice { get; set; }
        public int ExperiencePointsCost { get; set; } = 0;
        public Attributes AttributeBonus { get; set; } = new Attributes();
        public string PreferredWeapon { get; set; }
        public List<Class> Reclasses { get; set; }
        
      

      

        /*
         *  TODO: reclass options
         *  Ranger, Barbarian, Swashbuckler, Armsman, Samurai
         */
        public static Class Fighter()
        {
            return new Class()
            {
                Name = "Fighter",
                Description = "Warriors are lethal combatants who can use any weapon and armor with ease, " +
                              "relying on their strength and endurance instead of mana. With a wide range " +
                              "of offensive and defensive skills, they are a versatile class suitable for " +
                              "any race. For beginners, we highly recommend choosing a Human Warrior, as " +
                              "their high hit points and straightforward playstyle make them an easy class to learn.",
                PreferredWeapon = DefineSkill.LongBlades().Name,
                Skills = new List<SkillList>()
                {
                  
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                Reclasses = new List<Class>()
                {
                    Ranger()
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Malleus",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            };
        }
        
           public static Class Ranger()
        {
            return new Class()
            {
                Name = "Ranger",
                Description = "",
                PreferredWeapon = "Long Sword",
                Skills = new List<SkillList>()
                {
                  
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Malleus",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
        }
           
        /*
         *  Transmutor (Changes physical material properties)
         *  Mutator (Changes the composition of their self through potions)
         *  Herbalist (Creates healing & buffing potions)
         *  Toxicologist (Creates toxins and powders)
         */
         #region Alchemist
        public static Class Alchemist()
        {
            return new Class()
            {
                Name = "Alchemist",
                Description = "",
                PreferredWeapon = DefineSkill.ShortBlades().Name,
                Skills = new List<SkillList>()
                {
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                Reclasses = new List<Class>()
                {
                    Transmutor(),
                    Herbalist(),
                    Toxicologist()
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Ithdrak",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            
            };
        }

        public static Class Transmutor()
        {
            return new Class()
            {
                Name = "Transmutor",
                Description = "",
                PreferredWeapon = DefineSkill.LongBlades().Name,
                Skills = new List<SkillList>()
                {
                  
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Ithdrak",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            
            };
        }

        public static Class Herbalist()
        {
            return new Class()
            {
                Name = "Herbalist",
                Description = "",
                PreferredWeapon = DefineSkill.Staff().Name,
                Skills = new List<SkillList>()
                {
                    AddSkill(DefineSkill.Staff().Name, 1, 75, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Ithdrak",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            
            };
        }

        public static Class Toxicologist()
        {
            return new Class()
            {
                Name = "Toxicologist",
                Description = "",
                PreferredWeapon = DefineSkill.Crossbow().Name,
                Skills = new List<SkillList>()
                {
                    AddSkill(DefineSkill.Crossbow().Name, 5, 75, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false), 
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Ithdrak",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            
            };
        }

        public static Class Mutator()
        {
            return new Class()
            {
                Name = "Mutator",
                Description = "",
                PreferredWeapon = DefineSkill.HandToHand().Name,
                Skills = new List<SkillList>()
                {
                    AddSkill(DefineSkill.HandToHand().Name, 1, 75, false), 
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                   
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                     
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Ithdrak",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            
            };
        }
        #endregion
        
        
        /*
        *  TODO: reclass options
        *  Assassin,  Bandit, Nightshade, Pirate, Ninja
        */
        public static Class Rogue()
        {
            return new Class()
            {
                Name = "Rogue",
                Description = "Rogues are masters of stealth and cunning, delivering devastating blows from the shadows " +
                              "and slipping away undetected. While they may not have the durability of a warrior, their " +
                              "agility and evasiveness make them difficult targets to hit. Rogues are also skilled at " +
                              "picking locks and pockets, setting and disarming traps, and applying poison to their " +
                              "blades. They are a versatile class that can handle both combat and non-combat situations " +
                              "with ease. Important attributes for rogues are Dexterity, Constitution, and Intelligence. " +
                              "While any race can train to be a rogue, the quick and nimble Mau excel in this class.",
                PreferredWeapon = DefineSkill.ShortBlades().Name,
                Skills = new List<SkillList>()
                {
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.LockPick().Name, 1, 0, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false),
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Malleus",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
        }
        
        /*
*  TODO: reclass options
*  witch/warlock, Illusionist, Enchanter, Mentalist, Conjuror, Invoker
*/
         public static Class Mage()
        {
            return new Class()
            {
                Name = "Mage",
                Description = "Mages are feared for their devastating spells and power, but their path to mastery is a " +
                              "slow and arduous journey. They struggle in close combat due to their focus on studying " +
                              "magic, but their abilities become increasingly powerful as they progress. Intelligence, " +
                              "Wisdom, and Dexterity are key attributes for Mages, and all races can train in magic, " +
                              "though Elves are often considered the most adept.",
                PreferredWeapon = DefineSkill.Staff().Name,
                Skills = new List<SkillList>()
                {
             
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false),
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Malleus",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
        }
       
         /*
*  TODO: reclass options
*  Crusader, Druid, Shaman, Priest, 
*/
         public static Class Cleric()
        {
            return new Class()
            {
                Name = "Cleric",
                Description = "<p>Clerics draw their power from the gods they worship, with their strength increasing in " +
                              "proportion to their devotion. They specialize in spells that heal and protect their " +
                              "allies, but they also possess formidable offensive magic that can rival even that of " +
                              "mages. Like warriors, clerics can wear any type of armor, making them versatile " +
                              "fighters in combat.</p> <p>To become a powerful cleric, it is essential to prioritize Wisdom, " +
                              "Intelligence, and Constitution. While any race can train to become a cleric, Dwarfs are " +
                              "known to excel in this class due to their natural affinity for resilience and their deep " +
                              "respect for their chosen deity.</p>",
                PreferredWeapon = "Short Sword",
                Skills = new List<SkillList>()
                {
             
                    AddSkill(DefineSkill.LongBlades().Name, 1, 0, false),
                    AddSkill(DefineSkill.ShortBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.Axe().Name, 1, 0, false),
                    AddSkill(DefineSkill.Flail().Name, 1, 0, false),
                    AddSkill(DefineSkill.Polearm().Name, 1, 0, false),
                    AddSkill(DefineSkill.Blunt().Name, 1, 0, false),
                    AddSkill(DefineSkill.Spear().Name, 1, 0, false),
                    AddSkill(DefineSkill.Staff().Name, 1, 0, false),
                    AddSkill(DefineSkill.Whip().Name, 1, 0, false),
                    AddSkill(DefineSkill.HandToHand().Name, 1, 0, false),
                    AddSkill(DefineSkill.Crafting().Name, 1, 0, false),
                    AddSkill(DefineSkill.Cooking().Name, 1, 0, false),
                    AddSkill(DefineSkill.Foraging().Name, 1, 0, false),
                    AddSkill(DefineSkill.Lore().Name, 9, 0, false),
                    AddSkill(DefineSkill.Elbow().Name, 2, 0, false),
                    AddSkill(DefineSkill.DirtKick().Name, 3, 0, false),
                    AddSkill(DefineSkill.Kick().Name, 4, 0, false),
                    AddSkill(DefineSkill.Crossbow().Name, 5, 0, false),
                    AddSkill(DefineSkill.Bows().Name, 5, 0, false),
                    AddSkill(DefineSkill.Parry().Name, 6, 0, false),
                    AddSkill(DefineSkill.FastHealing().Name, 7, 0, false),
                    AddSkill(DefineSkill.ShieldBlock().Name, 8, 0, false),
                    AddSkill(DefineSkill.Charge().Name, 11, 0, false),
                    AddSkill(DefineSkill.Rescue().Name, 12, 0, false),
                    AddSkill(DefineSkill.UpperCut().Name, 14, 0, false),
                    AddSkill(DefineSkill.Trip().Name, 15, 0, false),
                    AddSkill(DefineSkill.Stab().Name, 17, 0, false),
                    AddSkill(DefineSkill.Mount().Name, 18, 0, false),
                    AddSkill(DefineSkill.SecondAttack().Name, 18, 0, false),
                    AddSkill(DefineSkill.Disarm().Name, 20, 0, false),
                    AddSkill(DefineSkill.EnhancedDamage().Name, 22, 0, false),
                    AddSkill(DefineSkill.LongBlades().Name, 1, 75, false),
                    AddSkill(DefineSkill.WarCry().Name, 25, 0, false),
                    AddSkill(DefineSkill.ShieldBash().Name, 26, 0, false),
                    AddSkill(DefineSkill.Lunge().Name, 28, 0, false),
                    AddSkill(DefineSkill.BlindFighting().Name, 29, 0, false),
                    AddSkill(DefineSkill.DualWield().Name, 31, 0, false),
                    AddSkill(DefineSkill.ThirdAttack().Name, 32, 0, false),
                    AddSkill(DefineSkill.HamString().Name, 35, 0, false),
                    AddSkill(DefineSkill.Slash().Name, 36, 0, false),
                    AddSkill(DefineSkill.Impale().Name, 37, 0, false),
                    AddSkill(DefineSkill.Cleave().Name, 40, 0, false),
                    AddSkill(DefineSkill.FourthAttack().Name, 42, 0, false),
                    AddSkill(DefineSkill.OverheadCrush().Name, 46, 0, false),
                    AddSkill(DefineSkill.FifthAttack().Name, 50, 0, false),
                },
                AttributeBonus = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        { EffectLocation.Strength, 2 },
                    }
                },
                HitDice = "1D10",
                ExperiencePointsCost = 1000,
                CreatedBy = "Malleus",
                Id = 1,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now
            };
        }
            
         public static List<Class> GetListOfClasses()
         {
             return new List<Class>()
             {
                Fighter(),
                Rogue(),
                Cleric(),
                Mage(),
                Alchemist()
             };
         }
        
         public static Class GetClassByName(string name)
         {
             return GetListOfClasses().FirstOrDefault(x => x.Name.Equals(name));
         }


         public static SkillList AddSkill(string skillName, int level, int startingProficiency, bool isSpell)
         {
             return new SkillList
             {
                 Level = level,
                 Proficiency = startingProficiency,
                 IsSpell = isSpell,
                 SkillName = skillName
             };
         }
              
                
        }
    }

  
