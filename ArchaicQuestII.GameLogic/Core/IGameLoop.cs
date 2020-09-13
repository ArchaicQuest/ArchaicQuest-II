using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IGameLoop
    {
          Task UpdateTime();
        Task UpdateRoomEmote();
        Task UpdateMobEmote();
        Task UpdatePlayers();
        Task UpdateCombat();
    }
}
