using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IGameLoop
    {
        void MainLoop();
        void UpdatePlayers();
    }
}
