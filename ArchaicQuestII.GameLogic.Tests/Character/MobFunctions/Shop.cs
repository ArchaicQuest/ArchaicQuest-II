using ArchaicQuestII.GameLogic.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Shop;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Character.MobFunctions
{
   
    public class ShopTests
    {

        private readonly Mock<IWriteToClient> _IWriteToClient;
        private readonly Mock<IUpdateClientUI> _IUpdateUI;
        private readonly Mock<IShop> _IShop;

        public ShopTests()
        {
            _IWriteToClient = new Mock<IWriteToClient>();
            _IUpdateUI = new Mock<IUpdateClientUI>();
            _IShop = new Mock<IShop>();
        }

        [Fact]
        public void Return_True_If_Shop_Keeper()
        {

            var shopkeeper = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Shopkeeper = true
            };

            var room = new Room();
            room.Mobs.Add(shopkeeper);


            var shop = new Shop(_IWriteToClient.Object, _IUpdateUI.Object);
            var mob = shop.FindShopKeeper(room);
            Assert.True(mob != null);
        }

        [Fact]
        public void List_Command_Only_Fires_If_Shopkeeper()
        {

            var item = new GameLogic.Item.Item()
            {
                Name = "Sword",

            };
            var shopkeeper = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Shopkeeper = false
            };

            shopkeeper.Inventory.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Player",
                Inventory = new ItemList(),
                Shopkeeper = false
            };

            var room = new Room();
            room.Mobs.Add(shopkeeper);


            var shop = new Shop(_IWriteToClient.Object, _IUpdateUI.Object);
           shop.List(room, player);


            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<p>There is no one selling here.</p>")), "1"), Times.Once());
 
        }

        [Fact]
        public void Display_Inventory()
        {

            var item = new GameLogic.Item.Item()
            {
                Name = "Sword",

            };
            var shopkeeper = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Shopkeeper = true
            };

            shopkeeper.Inventory.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Player",
                Inventory = new ItemList(),
                Shopkeeper = false
            };

            var room = new Room();
            room.Mobs.Add(shopkeeper);


            var shop = new Shop(_IWriteToClient.Object, _IUpdateUI.Object);
            shop.DisplayInventory(shopkeeper, player);


            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Gary says 'Here's what I have for sale.'")), "1"), Times.Once());

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<table class='data'><tr><td style='width: 30px; text-align: center;'>#</td><td style='width: 30px; text-align: center;'>Level</td><td  style='width: 100px;'>Price</td><td>Item</td></tr><tr><td style='width: 30px; text-align: center;'>1</td><td style='width: 30px; text-align: center;'>0</td><td  style='width: 100px;'>0 SP</td><td>Sword</td></tr></table>")), "1"), Times.Once());
        }

    }
}
