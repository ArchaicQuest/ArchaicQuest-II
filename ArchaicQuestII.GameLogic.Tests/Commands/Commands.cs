using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

namespace ArchaicQuestII.GameLogic.Tests.Commands
{
    public class CommandsTests
    {
        private GameLogic.World.Room.Room _room;
        private GameLogic.Commands.Commands _commands;
        private Player _player;
        private readonly Mock<IMovement> _movement;
        private readonly Mock<IRoomActions> _roomActions;
        private readonly Mock<IDebug> _debug;

        public CommandsTests()
        {
            _movement = new Mock<IMovement>();
            _roomActions = new Mock<IRoomActions>();
            _debug = new Mock<IDebug>();

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

            _commands = new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object, _debug.Object);

        }

        [Fact]
        public void Should_call_process_command()
        {

            _movement.Setup(x => x.Move(_room, _player, "North"));

            _commands.ProcessCommand(" NoRtH ", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North"), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "North East"));

            _commands.CommandList("ne", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "North West"));

            _commands.CommandList("nw", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North West"), Times.Once);

        }


        [Fact]
        public void Should_call_move_south_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "South West"));

            _commands.CommandList("sw", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South West"), Times.Once);

        }

        [Fact]
        public void Should_call_move_south_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "South East"));

            _commands.CommandList("se", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South East"), Times.Once);

        }



        [Fact]
        public void Should_call_move_north()
        {
       
            _movement.Setup(x => x.Move(_room, _player, "North"));

            _commands.CommandList("n", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North"), Times.Once);
        
        }

        [Fact]
        public void Should_call_move_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "East"));

            _commands.CommandList("e", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_South()
        {

            _movement.Setup(x => x.Move(_room, _player, "South"));

            _commands.CommandList("s", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Up()
        {

            _movement.Setup(x => x.Move(_room, _player, "Up"));

            _commands.CommandList("u", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Up"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Down()
        {

            _movement.Setup(x => x.Move(_room, _player, "Down"));

            _commands.CommandList("d", string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Down"), Times.Once);

        }

        [Fact]
        public void Should_call_debug()
        {

            _debug.Setup(x => x.DebugRoom(_room, _player));

            _commands.CommandList("/debug", string.Empty, _player, _room);

            _debug.Verify(x => x.DebugRoom(_room, _player), Times.Once);

        }


    }
}
