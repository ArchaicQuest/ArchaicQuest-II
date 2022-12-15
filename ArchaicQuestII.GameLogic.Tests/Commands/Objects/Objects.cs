using System;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands.Objects
{
    public class Objects
    {
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<ICore> _core;


        public Objects()
        {
            _writer = new Mock<IWriteToClient>();
            _core = new Mock<ICore>();
        }


        [Fact]
        public void Remove_Item_from_room()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description
                {
                    Room = "apple"
                }
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get apple");

            Assert.True(room.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }


        [Fact]
        public void Get_all_from_room()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description()
                {
                    Room = "apple"
                }
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get all");

            Assert.True(room.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }

        [Fact]
        public void Get_item_container()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description()
                {
                    Room = "apple"
                }
            };

            var chest = new GameLogic.Item.Item
            {
                Name = "chest",
                ItemType = GameLogic.Item.Item.ItemTypes.Container,
                Container = new Container { Items = new ItemList { apple }, IsOpen = true }
            };

            var room = new Room();
            room.Items.Add(chest);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get apple chest");

            Assert.True(chest.Container.Items.FirstOrDefault(x => x.Name == "apple") == null);
        }

        [Fact]
        public void Get_item_container_weight_of_item_should_not_be_that_of_the_container()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description
                {
                    Room = "apple"
                },
                Weight = 0.5F
            };

            var chest = new GameLogic.Item.Item
            {
                Name = "chest",
                Container = new Container { Items = new ItemList { apple }, IsOpen = true },
                Weight = 5,
                ItemType = GameLogic.Item.Item.ItemTypes.Container
            };

            var room = new Room();
            room.Items.Add(chest);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Weight = 0
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get apple chest");

            Assert.True(player.Inventory.FirstOrDefault(x => x.Name == "apple").Weight == 0.5);
        }



        [Fact]
        public void Add_item_to_inventory()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description
                {
                    Room = "apple"
                }
            };

            var room = new Room();
            room.Items.Add(apple);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get apple");

            Assert.True(player.Inventory.FirstOrDefault(x => x.Name == "apple") != null);
        }

        [Fact]
        public void Add_Gold_to_player()
        {
            var item = new GameLogic.Item.Item
            {
                Name = "gold",
                Description = new Description { Room = "gold" },
                Value = 5,
                ItemType = GameLogic.Item.Item.ItemTypes.Money
            };

            var room = new Room();
            room.Items.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Money = new GameLogic.Character.Model.Money()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "get gold");

            Assert.True(player.Money.Gold.Equals(5));
        }

        [Fact]
        public void Drop_Gold()
        {

            var room = new Room();

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Money = new GameLogic.Character.Model.Money
                {
                    Gold = 500,
                    Silver = 0
                }
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "drop 250 gold");

            Assert.True(room.Items.FirstOrDefault(x => x.Name.Contains("Gold", StringComparison.CurrentCultureIgnoreCase)) != null);
            Assert.True(player.Money.Gold.Equals(250));
        }

        [Fact]
        public void Give_gold_to_player()
        {

            var room = new Room();

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList(),
                Money = new GameLogic.Character.Model.Money()
                {
                    Gold = 500,
                    Silver = 0
                }
            };

            var playerB = new Player
            {
                ConnectionId = "2",
                Name = "Barry",
                Inventory = new ItemList(),
                Money = new GameLogic.Character.Model.Money()
                {
                    Gold = 0,
                    Silver = 0
                }
            };

            room.Players.Add(playerB);
            room.Players.Add(player);

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "give barry 500 gold");

            Assert.True(player.Money.Gold.Equals(0));
            Assert.True(playerB.Money.Gold.Equals(500));
        }

        [Fact]
        public void give_item_to_mob()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description
                {
                    Room = "apple"
                }
            };

            var room = new Room();

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList { apple }
            };

            var mob = new Player
            {
                ConnectionId = "mob",
                Name = "Mob",
                Inventory = new ItemList()
            };

            room.Players.Add(player);
            room.Players.Add(mob);

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "give Mob apple");

            Assert.True(player.Inventory.FirstOrDefault(x => x.Name == "apple") == null);
            Assert.True(mob.Inventory.FirstOrDefault(x => x.Name == "apple") != null);
        }

        [Fact]
        public void show_error_if_item_not_found_to_give()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description()
                {
                    Room = "apple"
                }
            };

            var room = new Room();

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList { apple }
            };

            var mob = new Player
            {
                ConnectionId = "mob",
                Name = "Mob",
                Inventory = new ItemList()
            };

            room.Players.Add(player);
            room.Players.Add(mob);

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "give Mob bread");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You do not have that item.")), "1"), Times.Once());
        }

        [Fact]
        public void show_error_if_char_not_found_to_give()
        {
            var apple = new GameLogic.Item.Item
            {
                Name = "apple",
                Description = new Description
                {
                    Room = "apple"
                }
            };

            var room = new Room();

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList { apple }
            };

            var mob = new Player
            {
                ConnectionId = "mob",
                Name = "Mob",
                Inventory = new ItemList()
            };

            room.Players.Add(player);
            room.Players.Add(mob);

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "give Max apple");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("They aren't here.")), "1"), Times.Once());
        }


        [Fact]
        public void Should_open_closed_door()
        {
            var exit = new Exit
            {
                Name = "North",
                Closed = true
            };
            var room = new Room
            {
                Exits =
                {
                    North = exit
                }
            };

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "open north");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You open the door")), "1"), Times.Once());
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


        //    var objects = new GameLogic.Commands.Objects.Object(_IWriteToClient.Object, _IUpdateUI.Object, _IMobScripts.Object,  _castSpell.Object);

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
                Container = new Container { CanOpen = true, IsOpen = false, Items = new ItemList() },

            };

            var room = new Room();
            room.Items.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "open chest");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You open")), "1"), Times.Once());
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
                Container = new Container { CanOpen = true, IsOpen = true, Items = new ItemList() },

            };

            var room = new Room();
            room.Items.Add(item);

            var player = new Player
            {
                ConnectionId = "1",
                Name = "Gary",
                Inventory = new ItemList()
            };

            new GameLogic.Commands.CommandHandler(_core.Object).HandleCommand(player, room, "close chest");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("You close")), "1"), Times.Once());
            Assert.True(!item.Container.IsOpen);
        }
    }
}
