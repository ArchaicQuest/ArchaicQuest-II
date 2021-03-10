using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Skill.Enum;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class CraftingSkills
    {
        public Model.Skill Cooking()
        {

            /// Note to self to rewrite the descriptions

            var coreSkill = new Model.Skill()
            {
                Name = "Cooking",
                Description =
                    "The art of cooking is an important skill to have, cooking certain foods allow you to boost your attributes beyond their maximum value or to replenish lost hit points or mana.",
                Type = SkillType.None,
                DateCreated = DateTime.Now,
                

            };


            return coreSkill;
        }

        public Model.Skill Crafting()
        {

            var coreSkill = new Model.Skill() {

                Name = "Crafting",
                Description =
                    "General crafting skill covers basic crafting items such as a camp fire or simple clothing that an adventurer can do that doesn't require an expertise in a particular field such as Armour or Weapon crafting.",
                Type = SkillType.None,
                DateCreated = DateTime.Now,
                


            };


            return coreSkill;
        }
    }
}
