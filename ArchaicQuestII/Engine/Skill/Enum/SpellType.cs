using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Skill.Enum
{
    public enum SkillType
    {
        None = 0,
        Affect = 1 << 0,  
        Travel = 1 << 1,
        Creation = 1 << 2,
        Summon = 1 << 3,
    }
}
