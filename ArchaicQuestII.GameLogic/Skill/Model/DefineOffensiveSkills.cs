using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Skill.Model;

public static class DefineOffensiveSkills
{
    public static Skill Kick()
    {
        return new Skill
        { 
            Name = "Kick",
           ManaCost = 0,
           MoveCost = 25
        };
    }
}