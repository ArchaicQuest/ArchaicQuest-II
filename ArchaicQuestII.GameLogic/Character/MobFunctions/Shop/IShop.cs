using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Shop
{
    public interface IShop
    {
        public void DisplayInventory(Player mob, Player player);
        public Player FindShopKeeper(Room room);
        public void List(Room room, Player player);

        public double AddMarkUp(int price);

        public string DisplayUnit(int price);


    }
}
