using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands.Movement
{
    public class MovementTests
    {
        private Room _room;
        private Player _player;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<CoreHandler> _core;
        private readonly Cache _cache;

        public MovementTests()
        {
            _writer = new Mock<IWriteToClient>();
            _cache = new Cache();
            _core = new Mock<CoreHandler>();
        }

        [Fact]
        public void Should_move_characters_position()
        {
            var player2 = new Player { ConnectionId = "2", Name = "Jane" };

            _player = new Player
            {
                ConnectionId = "1",
                Name = "Bob",
                Stats = new Stats { MovePoints = 110 },
                Attributes = new Attributes { Attribute = { [EffectLocation.Moves] = 100 } }
            };

            _room = new Room
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
                Exits = new ExitDirections
                {
                    North = new Exit
                    {
                        AreaId = 1,
                        RoomId = 2,
                        Name = "North",
                        Coords = new Coordinates
                        {
                            X = 0,
                            Y = 1,
                            Z = 0
                        },
                        Door = false,
                        Closed = false
                    }
                },
                Players = new List<Player> { _player, player2 }
            };

            var room2 = new Room
            {
                AreaId = 1,
                Id = 2,
                Title = "Room 2",
                Description = "room 2",
                Coords = new Coordinates
                {
                    X = 0,
                    Y = 1,
                    Z = 0
                },
                Exits = new ExitDirections
                {
                    South = new Exit
                    {
                        AreaId = 1,
                        Name = "South",
                        Door = false,
                        Closed = false,
                        Coords = new Coordinates
                        {
                            X = 0,
                            Y = 0,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
            };

            _cache.AddRoom(
                $"{room2.AreaId}{room2.Coords.X}{room2.Coords.Y}{room2.Coords.Z}",
                room2
            );
            _cache.AddRoom(
                $"{_room.AreaId}{_room.Coords.X}{_room.Coords.Y}{_room.Coords.Z}",
                _room
            );

            _player.EnterEmote = "";
            new GameLogic.Commands.CommandHandler().HandleCommand(_player, _room, "North");

            //TODO: Need to change this to use the new room actions RoomChange() method
            //_writer.Verify(w => w.WriteLine(It.Is<string>(s => s.Contains("Bob walks north.")), "1"), Times.Never);
            //_writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "<span class='player'>Bob walks north.</span>"), "2"), Times.Once);
            Assert.Equal("1010", _player.RoomId);
        }

        [Fact]
        public void Should_not_move_if_no_moves()
        {
            var player2 = new Player { ConnectionId = "2" };

            _player = new Player
            {
                ConnectionId = "1",
                Name = "Bob",
                Stats = new Stats() { MovePoints = 0 },
                Attributes = new Attributes { Attribute = { [EffectLocation.Moves] = 0 } }
            };

            _room = new Room()
            {
                AreaId = 1,
                Title = "Room 1",
                Description = "room 1",
                Exits = new ExitDirections()
                {
                    North = new Exit() { AreaId = 2, Name = "North" }
                },
                Players = new List<Player>() { _player, player2 }
            };

            var room2 = new Room()
            {
                AreaId = 2,
                Title = "Room 2",
                Description = "room 2",
                Exits = new ExitDirections()
                {
                    South = new Exit() { AreaId = 1, Name = "South" }
                },
                Players = new List<Player>()
            };

            //_cache.Setup(x => x.GetRoom(2)).Returns(room2);
            _cache.AddRoom(
                $"{room2.AreaId}{room2.Coords.X}{room2.Coords.Y}{room2.Coords.Z}",
                room2
            );

            //_cache.AddRoom(1, _room);

            new GameLogic.Commands.CommandHandler().HandleCommand(_player, _room, "North");

            _writer.Verify(
                w =>
                    w.WriteLine(
                        It.Is<string>(s => s == "<p>You are too exhausted to move.</p>"),
                        "1"
                    ),
                Times.Once
            );
        }

        [Fact]
        public void Should_return_error_if_room_not_found()
        {
            var player2 = new Player { ConnectionId = "2" };

            _player = new Player
            {
                ConnectionId = "1",
                Name = "Bob",
                Stats = new Stats { MovePoints = 110 },
                Attributes = new Attributes { Attribute = { [EffectLocation.Moves] = 100 } }
            };

            _room = new Room
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
                Exits = new ExitDirections
                {
                    North = new Exit
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
                Players = new List<Player> { _player, player2 }
            };

            var room2 = new Room
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
                Exits = new ExitDirections
                {
                    South = new Exit
                    {
                        AreaId = 1,
                        Name = "South",
                        Coords = new Coordinates
                        {
                            X = 0,
                            Y = 0,
                            Z = 0
                        }
                    }
                },
                Players = new List<Player>()
            };

            _cache.AddRoom("1020", room2);

            new GameLogic.Commands.CommandHandler().HandleCommand(_player, _room, "North");

            _writer.Verify(
                w =>
                    w.WriteLine(
                        It.Is<string>(
                            s => s == "<p>A mysterious force prevents you from going that way.</p>"
                        ),
                        "1"
                    ),
                Times.Once
            );
        }

        [Fact]
        public void Should_sit_down()
        {
            var player2 = new Player { ConnectionId = "2", Id = new Guid() };

            _player = new Player
            {
                Id = Guid.NewGuid(),
                ConnectionId = "1",
                Name = "Bob",
                Stats = new Stats { MovePoints = 110 }
            };

            _room = new Room
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
                Exits = new ExitDirections
                {
                    North = new Exit
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
                Players = new List<Player> { _player, player2 }
            };

            new GameLogic.Commands.CommandHandler().HandleCommand(_player, _room, "sit");
            _writer.Verify(
                w => w.WriteLine(It.Is<string>(s => s == "<p>You sit down.</p>"), "1"),
                Times.Once
            );
            Assert.Equal(CharacterStatus.Status.Sitting, _player.Status);
        }
    }
}
