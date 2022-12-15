using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Gain;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Healer;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Socials;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using Xunit;

//TODO: Fix
namespace ArchaicQuestII.GameLogic.Tests.Commands
{
    public class CommandsTests
    {
        private Room _room;
        private Player _player;
        private ICommandHandler _commandHandler;
        private readonly Mock<ICommand> _commandMove;
        private readonly Mock<ICore> _core;

        public CommandsTests()
        {
            _core = new Mock<ICore>();
            _player = new Player();
            _commandMove = new Mock<ICommand>();

            _player.ConnectionId = "1";
            _player.Name = "Bob";

            _room = new Room
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

            _commandHandler = new CommandHandler(_core.Object);
        }

        [Fact]
        public void Should_call_process_command()
        {

            //_movement.Setup(x => x.Move(_room, _player, "North", false, false));
            
            _commandHandler.HandleCommand(_player, _room, "NoRth");

            //_movement.Verify(x => x.Move(_room, _player, "North", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_east()
        {

            //_movement.Setup(x => x.Move(_room, _player, "North East", false, false));
            
            _commandHandler.HandleCommand(_player, _room, "ne");

            //_movement.Verify(x => x.Move(_room, _player, "North East", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_west()
        {

            //_movement.Setup(x => x.Move(_room, _player, "North West", false, false));

            _commandHandler.HandleCommand(_player, _room, "nw");

            //_movement.Verify(x => x.Move(_room, _player, "North West", false, false), Times.Once);

        }


        [Fact]
        public void Should_call_move_south_west()
        {

            //_movement.Setup(x => x.Move(_room, _player, "South West", false, false));

            _commandHandler.HandleCommand(_player, _room, "sw");

            //_movement.Verify(x => x.Move(_room, _player, "South West", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_south_east()
        {

            //_movement.Setup(x => x.Move(_room, _player, "South East", false, false));

            _commandHandler.HandleCommand(_player, _room, "se");

            //_movement.Verify(x => x.Move(_room, _player, "South East", false, false), Times.Once);

        }



        [Fact]
        public void Should_call_move_north()
        {

            //_movement.Setup(x => x.Move(_room, _player, "North", false, false));

            _commandHandler.HandleCommand(_player, _room, "n");

            //_movement.Verify(x => x.Move(_room, _player, "North", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_east()
        {

            //_movement.Setup(x => x.Move(_room, _player, "East", false, false));

            _commandHandler.HandleCommand(_player, _room, "e");

            //_movement.Verify(x => x.Move(_room, _player, "East", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_South()
        {

            //_movement.Setup(x => x.Move(_room, _player, "South", false, false));

            _commandHandler.HandleCommand(_player, _room, "s");

            //_movement.Verify(x => x.Move(_room, _player, "South", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_Up()
        {

            //_movement.Setup(x => x.Move(_room, _player, "Up", false, false));

            _commandHandler.HandleCommand(_player, _room, "u");

            //_movement.Verify(x => x.Move(_room, _player, "Up", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_Down()
        {

            _commandMove.Setup(x => x.Execute(_player, _room, new[]{"d"}));

            _commandHandler.HandleCommand(_player, _room, "d");

            _commandMove.Verify(x => x.Execute(_player, _room, new[]{"d"}), Times.Once);

        }

        [Fact]
        public void Should_call_debug()
        {
            //_commandHandler.Setup(x => x.HandleCommand(_player, _room, "/debug"));
            
            //_commandHandler.HandleCommand(_player, _room, "/debug");

            //_commandHandler.Verify(x => x.HandleCommand(_player, _room, "/debug"), Times.Once);

        }
    }
}
