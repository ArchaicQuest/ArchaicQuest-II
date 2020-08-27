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
                Description =
                    "In the words of one wise warrior, 'the best way to block a blow is to not\r\nbe where it lands'.  The dodge skill honors this tradition, by improving the\r\ncharacter's natural agility to the point where many blows will miss the \r\ntarget. The chance of dodging is also affected by the dexterity of the\r\nattacker and the target.  Any class may learn dodging..",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,
             
            };

            return coreSkill;
        }

        public Model.Skill ShieldBLock()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Shield Block",
                Description = "Shield block is a rather fancy name for the art of parrying with a shield.\r\nCharacters with no shield block skill will not be able to defend themselves\r\nwell with a shield.  All classes may learn shield block, but only warriors and\r\nclerics are good at it.  Beware, flails ignore shield blocking attempts, and\r\nwhips have an easier time getting around them.  Axes may split shields in two.\r\nShield block now works against charges from other rooms.\r\n",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }

        public Model.Skill Parry()
        {

            var coreSkill = new Model.Skill()
            {
                Name = "Parry",
                Description = "If at first you fail to dodge, block it.  Parry is useful for deflecting \r\nattacks, and is successful more often than dodge.  Parry requires a weapon for\r\nfull success, the hand-to-hand skill may also be used, but results in reduced\r\ndamage instead of no damage.  The best chance of parrying occurs when the\r\ndefender is skilled in both his and his opponent's weapon type.\r\n",
                Type = SkillType.Passive,
                DateCreated = DateTime.Now,

            };

            return coreSkill;
        }
 


    }
}

