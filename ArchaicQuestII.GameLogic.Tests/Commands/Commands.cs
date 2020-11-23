using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Commands.Communication;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Commands.Inventory;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Commands.Score;
using ArchaicQuestII.GameLogic.Commands.Skills;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Socials;
using ArchaicQuestII.GameLogic.Spell.Interface;
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
        private readonly Mock<ISkills> _skill;
        private readonly Mock<ISpells> _spell;
        private readonly Mock<IObject> _object;
        private readonly Mock<IInventory> _inventory;
        private readonly Mock<Icommunication> _communication;
        private readonly Mock<IEquip> _equipment;
        private readonly Mock<IScore> _score;
        private readonly Mock<ICombat> _combat;
        private readonly Mock<ICommandHandler> _commandHandler;
        private readonly Mock<ICache> _cache;
        private readonly Mock<ISocials> _socials;
        private readonly Mock<ICore> _core;

        public CommandsTests()
        {
            _movement = new Mock<IMovement>();
            _roomActions = new Mock<IRoomActions>();
            _debug = new Mock<IDebug>();
            _skill = new Mock<ISkills>();
            _spell = new Mock<ISpells>();
            _object = new Mock<IObject>();
            _inventory = new Mock<IInventory>();
            _communication = new Mock<Icommunication>();
            _equipment = new Mock<IEquip>();
            _score = new Mock<IScore>();
            _combat = new Mock<ICombat>();
            _commandHandler = new Mock<ICommandHandler>();
            _cache = new Mock<ICache>();
            _socials = new Mock<ISocials>();
            _core = new Mock<ICore>();
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

            _commands = new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object, _debug.Object, _skill.Object, _spell.Object, _object.Object, _inventory.Object, _communication.Object, _equipment.Object, _score.Object, _combat.Object, _cache.Object, _socials.Object, _commandHandler.Object, _core.Object);

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

            _commands.CommandList("ne", string.Empty, String.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "North West"));

            _commands.CommandList("nw", string.Empty, String.Empty,  _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North West"), Times.Once);

        }


        [Fact]
        public void Should_call_move_south_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "South West"));

            _commands.CommandList("sw", string.Empty, String.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South West"), Times.Once);

        }

        [Fact]
        public void Should_call_move_south_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "South East"));

            _commands.CommandList("se", string.Empty, String.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South East"), Times.Once);

        }



        [Fact]
        public void Should_call_move_north()
        {
       
            _movement.Setup(x => x.Move(_room, _player, "North"));

            _commands.CommandList("n", string.Empty, string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North"), Times.Once);
        
        }

        [Fact]
        public void Should_call_move_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "East"));

            _commands.CommandList("e", string.Empty, string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "East"), Times.Once);

        }

        [Fact]
        public void Should_call_move_South()
        {

            _movement.Setup(x => x.Move(_room, _player, "South"));

            _commands.CommandList("s", string.Empty, string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Up()
        {

            _movement.Setup(x => x.Move(_room, _player, "Up"));

            _commands.CommandList("u", string.Empty, string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Up"), Times.Once);

        }

        [Fact]
        public void Should_call_move_Down()
        {

            _movement.Setup(x => x.Move(_room, _player, "Down"));

            _commands.CommandList("d", string.Empty, string.Empty, _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Down"), Times.Once);

        }

        [Fact]
        public void Should_call_debug()
        {

            _debug.Setup(x => x.DebugRoom(_room, _player));

            _commands.CommandList("/debug", string.Empty, string.Empty,  _player, _room);

            _debug.Verify(x => x.DebugRoom(_room, _player), Times.Once);

        }


    }
}
