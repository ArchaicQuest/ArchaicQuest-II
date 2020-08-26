namespace ArchaicQuestII.GameLogic.Skill.Enum
{
    public enum SkillType
    {
        None = 0,
        Affect = 1 << 0,  
        Travel = 1 << 1,
        Creation = 1 << 2,
        Summon = 1 << 3,
        Passive = 1 << 4
    }
}
