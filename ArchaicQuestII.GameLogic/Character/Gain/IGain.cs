using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Class;

namespace ArchaicQuestII.GameLogic.Character.Gain
{
    public interface IGain
    {
        public void GainExperiencePoints(Player player, Player target);

        public void GainExperiencePoints(Player player, int value, bool showMessage);

        public void GainLevel(Player player);

        public void GainSkillExperience(Player character, int expGain, SkillList skill, int increase);
    }

}
