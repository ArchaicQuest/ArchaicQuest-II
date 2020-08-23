using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands.Movement
{
    public class MovementTests
    {
        private GameLogic.World.Room.Room _room;
        private Player _player;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IRoomActions> _roomActions;
        private readonly Mock<IUpdateClientUI> _clientui;
        private readonly Cache _cache;

        public MovementTests()
        {
            _writer = new Mock<IWriteToClient>();
            _cache =  new Cache();
            _roomActions = new Mock<IRoomActions>();
            _clientui = new Mock<IUpdateClientUI>();
        }

        [Fact]
        public void Should_move_characters_position()
        {
            var player2 = new Player();
            player2.ConnectionId = "2";
            player2.Name = "Jane";

            _player = new Player();
            _player.ConnectionId = "1";
            _player.Name = "Bob";
            _player.Stats = new Stats()
            {
                MovePoints = 110
            };

            _room = new Room()
            {
                AreaId = 1,
                Id= 1,
                Title = "Room 1",
                Description = "room 1",
                Coords = new Coordinates {
                            X = 0,
                            Y = 0,
                            Z = 0
                        },
                Exits = new ExitDirections()
                {
                    
                    North = new Exit()
                    {
                        AreaId = 1,
                        RoomId = 2,
                        Name = "North",
                        Coords = new Coordinates
                        {
                            X = 0,
                            Y = 1,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
                {
                    _player,
             
                    player2
                }
            };

            var room2 = new Room()
            {
                AreaId = 1,
                Id = 2,
                Title = "Room 2",
                Description = "room 2",
                Coords = new Coordinates {
                            X = 0,
                            Y = 1,
                            Z = 0
                        },
                Exits = new ExitDirections()
                {
                    South = new Exit()
                    {
                        AreaId = 1,
                        Name = "South",
                        Coords =
                        new Coordinates {
                            X = 0,
                            Y = 0,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
            };

            var newRoomCoords = new Coordinates
            {
                X = 0,
                Y = 1,
                Z = 0
            };


            _cache.AddRoom(2, room2);
            _cache.AddRoom(1, _room);
           
            new GameLogic.Commands.Movement.Movement(_writer.Object, _cache, _roomActions.Object, _clientui.Object).Move(_room, _player, "North");

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Bob walks north.")), "1"), Times.Never);
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "Bob walks north."), "2"), Times.Once);
            Assert.Equal(2, _player.RoomId);
        }

        [Fact]
        public void Should_not_move_if_no_moves()
        {
            var player2 = new Player();
            player2.ConnectionId = "2";

            _player = new Player();
            _player.ConnectionId = "1";
            _player.Name = "Bob";
            _player.Stats = new Stats()
            {
                MovePoints = 0
            };

            _room = new Room()
            {
                AreaId = 1,
                Title = "Room 1",
                Description = "room 1",
                Exits = new ExitDirections()
                {
                    North = new Exit()
                    {
                        AreaId = 2,
                        Name = "North"
                    }
                },
                Players = new List<Player>()
                {
                    _player,

                    player2
                }
            };

            var room2 = new Room()
            {
                AreaId = 2,
                Title = "Room 2",
                Description = "room 2",
                Exits = new ExitDirections()
                {
                    South = new Exit()
                    {
                        AreaId = 1,
                        Name = "South"
                    }
                },
                Players = new List<Player>()
            };

            //  _cache.Setup(x => x.GetRoom(2)).Returns(room2);
            _cache.AddRoom(2, room2);
           // _cache.AddRoom(1, _room);

            new GameLogic.Commands.Movement.Movement(_writer.Object, _cache, _roomActions.Object, _clientui.Object).Move(_room, _player, "North");

 
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "You are too exhausted to move"), "1"), Times.Once);
            
        }


        [Fact]
        public void Should_return_error_if_room_not_found()
        {
            var player2 = new Player();
            player2.ConnectionId = "2";

            _player = new Player();
            _player.ConnectionId = "1";
            _player.Name = "Bob";
            _player.Stats = new Stats()
            {
                MovePoints = 110
            };

            _room = new Room()
            {
                AreaId = 1,
                Id = 1,
                Title = "Room 1",
                Description = "room 1",
                Coords = new Coordinates
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                Exits = new ExitDirections()
                {

                    North = new Exit()
                    {
                        AreaId = 1,
                        RoomId = 2,
                        Name = "North",
                        Coords = new Coordinates
                        {
                            X = 0,
                            Y = 1,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
                {
                    _player,

                    player2
                }
            };

            var room2 = new Room()
            {
                AreaId = 1,
                Id = 2,
                Title = "Room 2",
                Description = "room 2",
                Coords = new Coordinates
                {
                    X = 0,
                    Y = 2,
                    Z = 0
                },
                Exits = new ExitDirections()
                {
                    South = new Exit()
                    {
                        AreaId = 1,
                        Name = "South",
                        Coords =
                        new Coordinates
                        {
                            X = 0,
                            Y = 0,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
            };

            _cache.AddRoom(2, room2);

            new GameLogic.Commands.Movement.Movement(_writer.Object, _cache, _roomActions.Object, _clientui.Object).Move(_room, _player, "North");


            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "A mysterious force prevents you from going that way."), "1"), Times.Once);

        }


    }
}
