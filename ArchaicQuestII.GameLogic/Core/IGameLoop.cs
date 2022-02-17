using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IGameLoop
   {
       // HP, Mana, Moves gain
       int GainAmount(int value, Player player);
       Task UpdateTime();
        Task UpdateRoomEmote();
        Task UpdateMobEmote();
        Task UpdatePlayers();
        Task UpdatePlayerLag();
        Task UpdateCombat();
        Task UpdateWorldTime();
        Task Tick();
        void IdleCheck(Player player);
   }
}
