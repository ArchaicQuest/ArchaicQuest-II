using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Gain
{
    public interface IGain
    {
        public void GainExperiencePoints(Player player, Player target);
        public void GainExperiencePoints(Player player, int value);
    }
}
