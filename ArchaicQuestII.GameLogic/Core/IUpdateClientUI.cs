using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IUpdateClientUI
   {
       /// <summary>
       /// Update HP UI
       /// </summary>
       /// <param name="player"></param>
      void UpdateHP(Player player);
       /// <summary>
       /// Update mana UI
       /// </summary>
       /// <param name="player"></param>
       void UpdateMana(Player player);
       /// <summary>
       /// Update moves UI
       /// </summary>
       /// <param name="player"></param>
       void UpdateMoves(Player player);
       /// <summary>
       /// Update Exp UI
       /// </summary>
       /// <param name="player"></param>
       void UpdateExp(Player player);

       void UpdateEquipment(Player player);

       void UpdateInventory(Player player);

       void UpdateScore(Player player);
       void UpdateCommunication(Player player, string message, string type);

   }
}
