using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Skill.Enum;

namespace ArchaicQuestII.Engine.Skill.Model
{
    public class SkillCost
    {
        public Dictionary<Cost, int> Table{ get; set; } = new Dictionary<Cost, int>
        {
            {Cost.None, 0},
            {Cost.HitPoints, 0},
            {Cost.Mana, 0},
            {Cost.Moves, 0},
        };
    }
}
