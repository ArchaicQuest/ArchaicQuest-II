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
                ShopKeeper = true
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
                ShopKeeper = false
            };

            shopkeeper.Inventory.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Player",
                Inventory = new ItemList(),
                ShopKeeper = false
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
                ShopKeeper = true
            };

            shopkeeper.Inventory.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Player",
                Inventory = new ItemList(),
                ShopKeeper = false
            };

            var room = new Room();
            room.Mobs.Add(shopkeeper);


            var shop = new Shop(_IWriteToClient.Object, _IUpdateUI.Object);
            shop.DisplayInventory(shopkeeper, player);


            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Gary says 'Here's what I have for sale.'")), "1"), Times.Once());

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("<table><tr><td>#</td><td>Name</td><td>Price</td></tr><tr><td>1</td><td>Sword</td><td>xx</td></tr></table>")), "1"), Times.Once());
        }

    }
}
