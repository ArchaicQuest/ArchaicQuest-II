﻿using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Character.MobFunctions.Shop
{
    public interface IShop
    {
        public void DisplayInventory(Player mob, Player player);
        public Player FindShopKeeper(Room room);
        public void List(Room room, Player player);
        public int Haggle(Player player, Player target);

        public double AddMarkUp(int price);

        public string DisplayUnit(int price, int hagglePriceReduction);

        public void InspectItem(int itemNumber, Room room, Player player);
        public void InspectItem(string itemName, Room room, Player player);

        public void BuyItem(int itemNumber, Room room, Player player);
        public void BuyItem(string itemName, Room room, Player player);
        public void SellItem(string itemName, Room room, Player player);
    }
}
