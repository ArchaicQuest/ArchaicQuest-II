using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands
{
    public class CommandsTests
    {
        private GameLogic.World.Room.Room _room;
        private Player _player;
        private readonly Mock<IMovement> _movement;
        private readonly Mock<IRoomActions> _roomActions;

        public CommandsTests()
        {
            _movement = new Mock<IMovement>();
            _roomActions = new Mock<IRoomActions>();

            _player = new Player();
            _player.ConnectionId = "1";
            _player.Name = "Bob";

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
                    _player
                }
            };

        }

        [Fact]
        public void Should_call_process_command()
        {

            _movement.Setup(x => x.Move(_room, _player, "North"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).ProcessCommand(" NoRtH ", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North"), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "North East"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("ne", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "North West"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("nw", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North West"), Times.Once);

        }


        [Fact]
        public void Should_call_move_south_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "South West"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("sw", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South West"), Times.Once);

        }

        [Fact]
        public void Should_call_move_south_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "South East"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("se", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South East"), Times.Once);

        }



        [Fact]
        public void Should_call_move_north()
        {
       
            _movement.Setup(x => x.Move(_room, _player, "North"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("n", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North"), Times.Once);
        
        }

        [Fact]
        public void Should_call_move_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "East"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("e", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_South()
        {

            _movement.Setup(x => x.Move(_room, _player, "South"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("s", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Up()
        {

            _movement.Setup(x => x.Move(_room, _player, "Up"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("u", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Up"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Down()
        {

            _movement.Setup(x => x.Move(_room, _player, "Down"));

            new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object).CommandList("d", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Down"), Times.Once);

        }



    }
}
