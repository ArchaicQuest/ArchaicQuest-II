using ArchaicQuestII.GameLogic.Skill.Model;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Spell.Interface
{
    public interface ISpellAction
    {
        void DisplayActionToUser(LevelBasedMessages levelBasedActions, List<Messages> Actions, int level);
    }
}
