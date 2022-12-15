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
        private readonly Mock<IRoomActions> _roomActions;
        private readonly Mock<ISpells> _spell;
        private readonly Mock<IEquip> _equipment;
        private readonly Mock<ICombat> _combat;
        private readonly Mock<ICache> _cache;
        private readonly Mock<ISocials> _socials;
        private readonly Mock<ICore> _core;
        private readonly Mock<IMobFunctions> _mobFunctions;
        private readonly Mock<IHelp> _help;
        private readonly Mock<IMobScripts> _mobScripts;
        private readonly Mock<ICrafting> _crafting;
        private readonly Mock<ICooking> _cooking;
        private readonly Mock<IUtilSkills> _utilSkills;
        private readonly Mock<IPassiveSkills> _passiveSkills;
        private readonly Mock<IHealer> _healer;
        private readonly Mock<IDamageSpells> _damageSpells;
        private readonly Mock<IGain> _gain;
        private readonly Mock<IAreaActions> _areaActions;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IUpdateClientUI> _clientui;

        public CommandsTests()
        {
            _roomActions = new Mock<IRoomActions>();
            _spell = new Mock<ISpells>();
            _equipment = new Mock<IEquip>();
            _combat = new Mock<ICombat>();
            _cache = new Mock<ICache>();
            _socials = new Mock<ISocials>();
            _core = new Mock<ICore>();
            _mobFunctions = new Mock<IMobFunctions>();
            _mobScripts = new Mock<IMobScripts>();
            _player = new Player();
            _help = new Mock<IHelp>();
            _crafting = new Mock<ICrafting>();
            _cooking = new Mock<ICooking>();
            _utilSkills = new Mock<IUtilSkills>();
            _passiveSkills = new Mock<IPassiveSkills>();
            _healer = new Mock<IHealer>();
            _damageSpells = new Mock<IDamageSpells>();
            _gain = new Mock<IGain>();
            _areaActions = new Mock<IAreaActions>();
            _writer = new Mock<IWriteToClient>();
            _clientui = new Mock<IUpdateClientUI>();
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

            _commandHandler = new CommandHandler(_cache.Object, _writer.Object, _clientui.Object, _roomActions.Object);
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
