using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Commands.Skills
{
    public interface ISkills
    {
        void ShowSkills(Player player, string fullCommand);
        void LearnMistakes(Player player, string skillName, int delay);
        bool SuccessCheck(Player player, string skillName);
    }
}
