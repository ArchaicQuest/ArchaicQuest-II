using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Item
{
   public interface IRandomItem
   {

       public Item WeaponDrop(Player player);
   }
}
