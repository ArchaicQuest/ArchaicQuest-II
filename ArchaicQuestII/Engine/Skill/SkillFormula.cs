using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Skill
{
    public class SkillFormula
    {
        public int CalculateValue(int level, Model.Skill skill, int mod)
        {
            return skill.Damage.Roll(skill.Damage.DiceRoll, skill.Damage.DiceMinSize,
                       skill.Damage.DiceMaxSize) + (level + 1) / 2 + mod;
        }  
    }
}
