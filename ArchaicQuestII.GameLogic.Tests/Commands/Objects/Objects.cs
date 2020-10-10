using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands.Objects
{
   public class Objects
   {
       private readonly Mock<IWriteToClient> _IWriteToClient;
       private readonly Mock<IUpdateClientUI> _IUpdateUI;

        public Objects()
       {
           _IWriteToClient = new Mock<IWriteToClient>();
           _IUpdateUI = new Mock<IUpdateClientUI>();

       }


        [Fact]
        public void Remove_Item_from_room()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Get("apple", "", room, player);


            Assert.True(room.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }


        [Fact]
        public void Get_all_from_room()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Get("all", "", room, player);


            Assert.True(room.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }

        [Fact]
        public void Get_item_container()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var chest = new GameLogic.Item.Item
            {
                Name = "chest",
                Container = new Container() {Items = new ItemList {apple}, IsOpen = true}
            };

            var room = new Room();
            room.Items.Add(chest);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Get("apple", "chest", room, player);


            Assert.True(chest.Container.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }



        [Fact]
        public void Add_item_to_inventory()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Get("apple", "", room, player);


            Assert.True(player.Inventory.FirstOrDefault(x => x.Name == "apple") != null);
        }

        [Fact]
        public void give_item_to_mob()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();
            player.Inventory.Add(apple);

            var mob = new Player();
            mob.ConnectionId = "mob";
            mob.Name = "Mob";
            mob.Inventory = new ItemList();

            room.Players.Add(player);
            room.Players.Add(mob);

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Give("apple", "mob", room, player);


            Assert.True(player.Inventory.FirstOrDefault(x => x.Name == "apple") == null);
            Assert.True(mob.Inventory.FirstOrDefault(x => x.Name == "apple") != null);
        }

        [Fact]
        public void show_error_if_item_not_found_to_give()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();
            player.Inventory.Add(apple);

            var mob = new Player();
            mob.ConnectionId = "mob";
            mob.Name = "Mob";
            mob.Inventory = new ItemList();

            room.Players.Add(player);
            room.Players.Add(mob);

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Give("bread", "mob", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You do not have that item.")), "1"), Times.Once());
        }

        [Fact]
        public void show_error_if_char_not_found_to_give()
        {
            var apple = new GameLogic.Item.Item();
            apple.Name = "apple";
            apple.Description = new Description()
            {
                Room = "apple"
            };

            var room = new Room();

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();
            player.Inventory.Add(apple);

            var mob = new Player();
            mob.ConnectionId = "mob";
            mob.Name = "Mob";
            mob.Inventory = new ItemList();

            room.Players.Add(player);
            room.Players.Add(mob);

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object);

            objects.Give("apple", "max", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("They aren't here.")), "1"), Times.Once());
        }
    }
}
