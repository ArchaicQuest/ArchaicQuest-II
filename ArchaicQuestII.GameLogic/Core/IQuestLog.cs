using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface IQuestLog
    {
        void IsQuestMob(Player player, string mobName);
    }
}
