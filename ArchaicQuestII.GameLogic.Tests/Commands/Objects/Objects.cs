using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.VisualStudio.TestPlatform.Common.DataCollection;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands.Objects
{
   public class Objects
   {
       private readonly Mock<IWriteToClient> _IWriteToClient;
       private readonly Mock<IUpdateClientUI> _IUpdateUI;
       private readonly Mock<IMobScripts> _IMobScripts;

        public Objects()
       {
           _IWriteToClient = new Mock<IWriteToClient>();
           _IUpdateUI = new Mock<IUpdateClientUI>();
           _IMobScripts = new Mock<IMobScripts>();

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

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

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

            objects.Give("apple", "max", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("They aren't here.")), "1"), Times.Once());
        }


        [Fact]
        public void Should_open_closed_door()
        {
            var exit = new Exit();
            exit.Name = "North";
           exit.Closed = true;
            var room = new Room();
            room.Exits.North = exit;

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();
             

            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

            objects.Open("north", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You open the door")), "1"), Times.Once());
        }

        //[Fact]
        //public void Should_close_open_door()
        //{
        //    var exit = new Exit();
        //    exit.Name = "North";
        //    exit.Closed = false;
        //    var room = new Room();
        //    room.Exits.North = exit;

        //    var player = new Player();
        //    player.ConnectionId = "1";
        //    player.Name = "Gary";
        //    player.Inventory = new ItemList();


        //    var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

        //    objects.Close("north", room, player);

        //    _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You close the door")), "1"), Times.Once());
        //}

        [Fact]
        public void Should_open_closed_Container()
        {
            var item = new GameLogic.Item.Item
            {
                Name = "Chest",
                Description = new Description(),
                
                ItemType = GameLogic.Item.Item.ItemTypes.Container,
                Container = new Container(){CanOpen = true, IsOpen = false, Items = new ItemList()},
                
            };

            var room = new Room();
            room.Items.Add(item);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();


            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

            objects.Open("chest", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You open")), "1"), Times.Once());
            Assert.True(item.Container.IsOpen);
        }

        [Fact]
        public void Should_close_opened_Container()
        {
            var item = new GameLogic.Item.Item
            {
                Name = "Chest",
                Description = new Description(),

                ItemType = GameLogic.Item.Item.ItemTypes.Container,
                Container = new Container() { CanOpen = true, IsOpen = true, Items = new ItemList() },

            };

            var room = new Room();
            room.Items.Add(item);

            var player = new Player();
            player.ConnectionId = "1";
            player.Name = "Gary";
            player.Inventory = new ItemList();


            var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object);

            objects.Close("chest", room, player);

            _IWriteToClient.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You close")), "1"), Times.Once());
            Assert.True(!item.Container.IsOpen);
        }
    }

}
