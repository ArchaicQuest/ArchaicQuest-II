using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.Equipment
{
   public interface IEquip
    {
        void Wear(string item, Room room, Player player);
        void Remove(string item, Room room, Player player);
        void ShowEquipment(Player player);
        string ShowEquipmentUI(Player player);
    }
}
