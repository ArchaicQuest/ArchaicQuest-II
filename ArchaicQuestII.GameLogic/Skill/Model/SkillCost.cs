using ArchaicQuestII.GameLogic.Skill.Enum;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Skill.Model
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
