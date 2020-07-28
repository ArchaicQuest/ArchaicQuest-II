using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IUpdateClientUI
   {
      void UpdateHP(Player player);
   }
}
