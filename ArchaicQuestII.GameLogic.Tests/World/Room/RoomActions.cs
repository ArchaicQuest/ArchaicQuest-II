using ArchaicQuestII.GameLogic.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.World.Room
{
    public class RoomActionsTests
    {
        private GameLogic.World.Room.Room _room;
        private Player _player;
        private readonly Mock<IWriteToClient> _writer;
 
        public RoomActionsTests()
        {
            _writer = new Mock<IWriteToClient>();
            
        }

        [Fact]
        public void Should_return_room_description()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Title = "Room 1",
                Description =
                           "room descriptions",
   
                Exits = new ExitDirections()
                {
                    North = new Exit()
                    {
                        Name = "North"
                    }
                }
            };

            _player = new Player();
            _player.ConnectionId = "1";

            new RoomActions(_writer.Object).Look("", _room, _player);

            var roomDesc = "<p class=\"room-title\">Room 1<br /></p>" +
                           "<p class=\"room-description\">room descriptions</p>" +
                           "<p></p>" +
                           "<p></p>" +
                           "<p></p>" +
                           "<p class=\"room-exit\"> <span class=\"room-exits\">[</span>Exits: <span class=\"room-exits\">North</span><span class=\"room-exits\">]</span></p>";

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == roomDesc), "1"), Times.Once);
        }

        [Fact]
        public void Should_return_north_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    North = new Exit()
                    {
                        Name = "North"
                    }
                }
            };

            var exits =  new RoomActions(_writer.Object).FindValidExits(_room);
        
            Assert.Equal("North", exits);
        }

        [Fact]
        public void Should_return_northEast_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthEast = new Exit()
                    {
                        Name = "North East"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("North East", exits);
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
                        Name = "East"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("East", exits);
        }

        [Fact]
        public void Should_return_south_east_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    SouthEast = new Exit()
                    {
                        Name = "South East"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("South East", exits);
        }

        [Fact]
        public void Should_return_south_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    South = new Exit()
                    {
                        Name = "South"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("South", exits);
        }

        [Fact]
        public void Should_return_south_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    SouthWest = new Exit()
                    {
                        Name = "South West"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("South West", exits);
        }

        [Fact]
        public void Should_return_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    West = new Exit()
                    {
                        Name = "West"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("West", exits);
        }

        [Fact]
        public void Should_return_north_west_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthWest = new Exit()
                    {
                        Name = "North West"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("North West", exits);
        }

        [Fact]
        public void Should_return_none_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("None", exits);
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
                        Name = " North West"
                    },

                    North = new Exit()
                    {
                        Name = " North"
                    },
                    NorthEast = new Exit()
                    {
                        Name = " North East"
                    },
                    East = new Exit()
                    {
                        Name = " East"
                    },
                    SouthEast = new Exit()
                    {
                        Name = " South East"
                    },
                    South = new Exit()
                    {
                        Name = " South"
                    },
                    SouthWest = new Exit()
                    {
                        Name = " South West"
                    },
                    West = new Exit()
                    {
                        Name = " West"
                    },
                   
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal(" North West,  North,  North East,  East,  South East,  South,  South West,  West", exits);
        }

        [Fact]
        public void Should_return_custom_exits()
        {
            _room = new GameLogic.World.Room.Room()
            {
                Exits = new ExitDirections()
                {
                    NorthWest = new Exit()
                    {
                        Name = "A hole in the wall"
                    }
                }
            };

            var exits = new RoomActions(_writer.Object).FindValidExits(_room);

            Assert.Equal("A hole in the wall", exits);
        }
    }
}
