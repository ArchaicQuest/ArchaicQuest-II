using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Skill.Enum;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class WeaponTypeSkills
    {
        public Model.Skill LongBlade()
        {
        

            var coreSkill = new Model.Skill()
            {
                Name = "Long Blades",
                Description =
                    "This skill allows the user proficiency in all blades that fall in the long blade category." +
                    "Such as Great Swords, Two Handed Swords, Sabre, and Longsword.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,
             
            };

            return coreSkill;
        }

        public Model.Skill Axe()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Axe",
                Description =  "This skill allows the user proficiency in Axes",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Blunt()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Blunt",
                Description = "This skill allows the user proficiency in Blunt weapons",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Bows()
        {
           
        var coreSkill = new Model.Skill()
            {
                Name = "Bows",
                Description = "This skill allows the user proficiency with Bows and Arrows",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Crossbow()
        {
         
            var coreSkill = new Model.Skill()
            {
                Name = "Crossbow",
                Description = "This skill allows the user proficiency with Crossbows",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Exotic()
        {
           
            var coreSkill = new Model.Skill()
            {
                Name = "Exotic",
                Description = "This skill allows the user proficiency in Exotic weapons that's everything from a Katana to a chair leg.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Flail()
        {
            
                var coreSkill = new Model.Skill()
                {
                    Name = "Flail",
                    Description = "This skill allows the user proficiency in Flail weapons.",
                    Type = SkillType.Passive,
                    DateCreated = DateTime.Now,

                };

            return coreSkill;
        }

        public Model.Skill HandToHand()
        {
            
            var coreSkill = new Model.Skill()
            {
                Name = "Hand to hand",
                Description = "This skill allows the user proficiency in hand to hand combat.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Polearm()
        { 
            var coreSkill = new Model.Skill()
            {
                Name = "Polearm",
                Description = "This skill allows the user proficiency in Polearm weapons.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill ShortBlades()
        {
          
            var coreSkill = new Model.Skill()
            {
                Name = "Short Blades",
                Description = "This skill allows the user proficiency in all blades that fall in the short blade category." +
                              "Such as short Swords, scimitar, and daggers",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }
        public Model.Skill Spear()
        {
        
            var coreSkill = new Model.Skill()
            {
                Name = "Spear",
                Description = "This skill allows the user proficiency in Spears.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Staff()
        {
            var coreSkill = new Model.Skill()
            {
                Name = "Staff",
                Description = "This skill allows the user proficiency in Staffs.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }


        public Model.Skill Whip()
        {
            var coreSkill = new Model.Skill()
            {
                Name = "Whip",
                Description = "This skill allows the user proficiency in Whips.",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }


    }
}

