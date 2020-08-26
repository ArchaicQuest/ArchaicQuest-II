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
    }
}

