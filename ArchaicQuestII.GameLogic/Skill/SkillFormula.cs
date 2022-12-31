using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Skill
{
    public class SkillFormula
    {
        public int CalculateValue(int level, Model.Skill skill, int mod)
        {
            return DiceBag.Roll(skill.Damage) + (level + 1) / 2 + mod;
        }
    }
}
