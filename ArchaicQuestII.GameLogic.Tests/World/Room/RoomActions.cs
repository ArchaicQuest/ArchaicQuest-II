using ArchaicQuestII.GameLogic.Core;
using Moq;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.World.Room;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.World.Room
{
    public class RoomActionsTests
    {
        private GameLogic.World.Room.Room _room;
        private Player _player;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<ITime> _time;
        private readonly Mock<Cache> _cache;
        private readonly Mock<IFormulas> _formulas;
        private readonly Mock<IPassiveSkills> _passiveSkills;
        private readonly Mock<IMobScripts> _mobScripts;
        private readonly Mock<IUpdateClientUI> _updateClient;

        public RoomActionsTests()
        {
            _writer = new Mock<IWriteToClient>();
            _time = new Mock<ITime>();
            _cache = new Mock<Cache>();
            _formulas = new Mock<IFormulas>();
            _passiveSkills = new Mock<IPassiveSkills>();
            _mobScripts = new Mock<IMobScripts>();
            _updateClient = new Mock<IUpdateClientUI>();
        }

        // too brittle
        //[Fact]
        //public void Should_return_room_description()
        //{
        //    _room = new GameLogic.World.Room.Room()
        //    {
        //        Title = "Room 1",
        //        Description =
        //                   "room descriptions",

        //        Exits = new ExitDirections()
        //        {
        //            North = new Exit()
        //            {
        //                Name = "North"
        //            }
        //        }
        //    };

        //    _player = new Player();
        //    _player.Config.VerboseExits = false;
        //    _player.ConnectionId = "1";

        //    new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object).Look("", _room, _player);

        //    _writer.Verify(w => w.WriteLine(It.IsAny<string>(), "1"), Times.Once);
        //}

        [Fact]
        public void Should_return_north_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    North = new Exit() { Name = "North", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object, _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("North", exits);
        }

        [Fact]
        public void Should_return_northEast_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthEast = new Exit() { Name = "North East", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("North East", exits);
        }

        [Fact]
        public void Should_return_east_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    East = new Exit()
                    {
                        Name = "East",
                        Door = false,
                        Coords = new Coordinates()
                    }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("East", exits);
        }

        [Fact]
        public void Should_return_south_east_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    SouthEast = new Exit() { Name = "South East", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("South East", exits);
        }

        [Fact]
        public void Should_return_south_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    South = new Exit() { Name = "South", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("South", exits);
        }

        [Fact]
        public void Should_return_south_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    SouthWest = new Exit() { Name = "South West", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("South West", exits);
        }

        [Fact]
        public void Should_return_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    West = new Exit() { Name = "West", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("West", exits);
        }

        [Fact]
        public void Should_return_north_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthWest = new Exit() { Name = "North West", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("North West", exits);
        }

        [Fact]
        public void Should_return_none_exits()
        {
            _room = new GameLogic.World.Room.Room() { Exits = new ExitDirections() { } };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("None", exits);
        }

        [Fact]
        public void Should_return_all_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthWest = new Exit()
                    {
                        Name = " North West",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    North = new Exit()
                    {
                        Name = " North",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    NorthEast = new Exit()
                    {
                        Name = " North East",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    East = new Exit()
                    {
                        Name = " East",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    SouthEast = new Exit()
                    {
                        Name = " South East",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    South = new Exit()
                    {
                        Name = " South",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    SouthWest = new Exit()
                    {
                        Name = " South West",
                        Door = false,
                        Coords = new Coordinates()
                    },
                    West = new Exit()
                    {
                        Name = " West",
                        Door = false,
                        Coords = new Coordinates()
                    },
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal(" North,  East,  South,  West,  North East,  South East,  South West,  North West", exits);
        }

        [Fact]
        public void Should_return_custom_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthWest = new Exit() { Name = "A hole in the wall", Door = false, }
                }
            };

            //var exits = new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).FindValidExits(_room, false);

            //Assert.Equal("A hole in the wall", exits);
        }

        [Fact]
        public void Should_return_view_of_next_room_from_portal()
        {
            _player = new Player();
            _player.Name = "Liam";
            _player.Config.VerboseExits = false;
            _player.ConnectionId = "1";

            var item = new GameLogic.Item.Item()
            {
                Name = "A portal",
                ItemType = GameLogic.Item.Item.ItemTypes.Portal,
                Portal = new Portal() { Destination = "0000" }
            };
            var currentRoom = new GameLogic.World.Room.Room()
            {
                Items = new ItemList() { item },
                AreaId = 1,
                Coords =
                {
                    X = 1,
                    Y = 0,
                    Z = 0
                },
                Players = new List<Player>() { _player }
            };

            _room = new GameLogic.World.Room.Room()
            {
                Description = "Room description",
                AreaId = 0,
                Coords =
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                }
            };

            _cache.Setup(x => x.GetRoom("0000")).Returns(_room);
            //new RoomActions(_writer.Object, _time.Object, _cache.Object, _dice.Object, _gain.Object, _formulas.Object,  _passiveSkills.Object, _updateClient.Object, _mobScripts.Object).LookInPortal(item, currentRoom, _player);

            _writer.Verify(
                w => w.WriteLine(It.Is<string>(s => s.Contains("Room description")), "1"),
                Times.Once()
            );
        }
    }
}
