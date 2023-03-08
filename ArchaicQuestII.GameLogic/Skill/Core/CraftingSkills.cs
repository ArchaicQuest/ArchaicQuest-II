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
            };


            return coreSkill;
        }

        public Model.Skill Crafting()
        {

            var coreSkill = new Model.Skill()
            {

                Name = "Crafting",

            };


            return coreSkill;
        }
        
        public Model.Skill Foraging()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Foraging",
            };


            return coreSkill;
        }
    }
}
