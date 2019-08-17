
using ArchaicQuestII.Engine.Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Skill.Model;

namespace ArchaicQuestII.Engine.Spell.Interface
{
    public interface ISpellAction
    {
        void DisplayActionToUser(LevelBasedMessages levelBasedActions, List<Messages> Actions, int level);
    }
}
