using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.Commands.Inventory
{
    public interface IInventory
    {
        public void List(Player player);
    }
}
