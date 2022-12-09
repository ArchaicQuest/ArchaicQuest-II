﻿using System;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Healer;
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
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Socials;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Area;
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
        private readonly Mock<IMobFunctions> _mobFunctions;
        private readonly Mock<IHelp> _help;
        private readonly Mock<IMobScripts> _mobScripts;
        private readonly Mock<ICrafting> _crafting;
        private readonly Mock<ICooking> _cooking;
        private readonly Mock<IUtilSkills> _utilSkills;
        private readonly Mock<IPassiveSkills> _passiveSkills;
        private readonly Mock<IHealer> _healer;
        private readonly Mock<IDamageSpells> _damageSpells;
        private readonly Mock<IAreaActions> _areaActions;
        
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
            _areaActions = new Mock<IAreaActions>();

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

            _commands = new GameLogic.Commands.Commands(_movement.Object, _roomActions.Object, _debug.Object, _skill.Object, _spell.Object, _object.Object, _inventory.Object, _communication.Object, _equipment.Object, _score.Object, _combat.Object, _cache.Object, _socials.Object, _commandHandler.Object, _core.Object, _mobFunctions.Object, _help.Object, _mobScripts.Object, _crafting.Object, _cooking.Object, _utilSkills.Object, _passiveSkills.Object, _healer.Object, _damageSpells.Object, _areaActions.Object);

        }

        [Fact]
        public void Should_call_process_command()
        {

            _movement.Setup(x => x.Move(_room, _player, "North", false, false));

            _commands.ProcessCommand("NoRtH ", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "North East", false, false));

            _commands.CommandList("ne", string.Empty, String.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North East", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_north_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "North West", false, false));

            _commands.CommandList("nw", string.Empty, String.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North West", false, false), Times.Once);

        }


        [Fact]
        public void Should_call_move_south_west()
        {

            _movement.Setup(x => x.Move(_room, _player, "South West", false, false));

            _commands.CommandList("sw", string.Empty, String.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South West", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_south_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "South East", false, false));

            _commands.CommandList("se", string.Empty, String.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South East", false, false), Times.Once);

        }



        [Fact]
        public void Should_call_move_north()
        {

            _movement.Setup(x => x.Move(_room, _player, "North", false, false));

            _commands.CommandList("n", string.Empty, string.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "North", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_east()
        {

            _movement.Setup(x => x.Move(_room, _player, "East", false, false));

            _commands.CommandList("e", string.Empty, string.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "East", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_South()
        {

            _movement.Setup(x => x.Move(_room, _player, "South", false, false));

            _commands.CommandList("s", string.Empty, string.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "South", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_Up()
        {

            _movement.Setup(x => x.Move(_room, _player, "Up", false, false));

            _commands.CommandList("u", string.Empty, string.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Up", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_move_Down()
        {

            _movement.Setup(x => x.Move(_room, _player, "Down", false, false));

            _commands.CommandList("d", string.Empty, string.Empty, "", _player, _room);

            _movement.Verify(x => x.Move(_room, _player, "Down", false, false), Times.Once);

        }

        [Fact]
        public void Should_call_debug()
        {

            _debug.Setup(x => x.DebugRoom(_room, _player));

            _commands.CommandList("/debug", string.Empty, string.Empty, "", _player, _room);

            _debug.Verify(x => x.DebugRoom(_room, _player), Times.Once);

        }


    }
}
