using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Skill.Enum;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class EvasiveTypeSkills
    {
        public Model.Skill Dodge()
        {

            /// Note to self to rewrite the descriptions

            var coreSkill = new Model.Skill()
            {
                Name = "Dodge",
            };

            return coreSkill;
        }

        public Model.Skill ShieldBLock()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Shield Block",
            };

            return coreSkill;
        }

        public Model.Skill Parry()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Parry",
            };

            return coreSkill;
        }



    }
}

