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
       
        public CommandsTests()
        {
            _movement = new Mock<IMovement>();
       
        }

        [Fact]
        public void Should_call_move_north()
        {
            var player2 = new Player();
            player2.ConnectionId = "2";

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
                    _player,
             
                    player2
                }
            };


            _movement.Setup(x => x.Move(_room, player2, "North"));


            new GameLogic.Commands.Commands(_movement.Object).CommandList("n", string.Empty, player2, _room);

            _movement.Verify(x => x.Move(_room, player2, "North"), Times.Once);
        
        }

       
    }
}
