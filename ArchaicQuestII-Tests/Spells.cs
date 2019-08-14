using ArchaicQuestII.Engine.Core.Events;
using System;
using System.Buffers;
using System.Collections.Generic;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Item;
using ArchaicQuestII.Engine.Skills;
using ArchaicQuestII.Engine.Spell;
using ArchaicQuestII.Engine.Spell.Model;
using ArchaicQuestII.Engine.World.Room.Model;
using Moq;
using Xunit;

namespace ArchaicQuestII_Tests
{
    public class SpellTests
    {
        private readonly Spell _Spells;
        private readonly Player _player;
        private readonly Player _target;
        private readonly Room _room;

        public SpellTests()
        {
           _Spells = new Spell()
            { 
                Name = "Ogre strength",
                Effect = new Effect()
                {
                    Modifier = new EffectModifer()
                    {
                        Value = 10,
                        PositiveEffect = true
                    },
                    Location = EffectLocation.Strength,
                    Name = "OgreStrength",
                    Duration = new EffectModifer()
                    {
                        Value = 10,
                        PositiveEffect = true,
                    },
                    Accumulate = true,
                    Id = 1
                },
                Damage = new Dice()
                {
                    DiceRoll = 1,
                    DiceMinSize = 1,
                    DiceMaxSize = 10
                },
                Description = "Makes you strong as an ogre",
                SkillAction = new List<Messages>
                {
                    new Messages
                    {
                        ToPlayer = "x",
                        ToRoom = "y",
                        ToTarget = "z"
                    }
                },
                Rounds = 1,
                SkillStart = new Messages()
                {
                    ToPlayer = "start spell",
                    ToRoom = "sy",
                    ToTarget = "sz"
                },
                Requirements = new Requirements()
                {
                    Evil = true
                },
                Type = new SpellType()
                {
                    Affect = true
                },
                Cost = Cost.HitPoints,
                LevelBasedMessages = new LevelBasedMessages(),
                SkillEnd = new Messages()
                {
                    ToPlayer = "sx",
                    ToRoom = "sy",
                    ToTarget = "sz"
                },
                SkillFailure = new Messages()
                {
                    ToPlayer = "sx",
                    ToRoom = "sy",
                    ToTarget = "sz"
                },
                SpellGroup = new Sphere()

            };

            _player = new Player();
           
            _target = new Player()
            {
                Attributes = new Attributes()
                {
                    Strength = 10
                }

            };

            _room = new Room();

        }
        [Fact]
        public void SpellAffectStrength()
        {
            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            Assert.True(_target.Attributes.Strength > 10, "The strength attribute was not greater than ten");
           
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "start spell")), Times.Once);
        }

        [Fact]
        public void SpellWontFireIfRoundIsHigherThanOne()
        {

            _Spells.Rounds = 2;
            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            Assert.True(_Spells.Rounds == 1, "Rounds remaining should equal 1");

          
        }

        [Fact]
        public void Should_show_level_based_message_for_level_ten()
        {

            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;
            _player.Level = 1;
            _Spells.LevelBasedMessages = new LevelBasedMessages()
            {
                HasLevelBasedMessages = true,
                Ten = new Messages()
                {
                    ToPlayer = "level ten"
                }
            };

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level ten")), Times.Once);


        }

        [Fact]
        public void Should_show_level_based_message_for_level_twenty()
        {

            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;
            _player.Level = 15;
            _Spells.LevelBasedMessages = new LevelBasedMessages()
            {
                HasLevelBasedMessages = true,
                Twenty = new Messages()
                {
                    ToPlayer = "level twenty",
                    ToTarget = "level twenty target",
                    ToRoom = "level twenty room"
                }
            };

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level twenty")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level twenty target")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level twenty room")), Times.Once);


        }

        [Fact]
        public void Should_show_level_based_message_for_level_Thirty()
        {

            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;
            _player.Level = 25;
            _Spells.LevelBasedMessages = new LevelBasedMessages()
            {
                HasLevelBasedMessages = true,
                Thirty = new Messages()
                {
                    ToPlayer = "level Thirty",
                    ToTarget = "level Thirty target",
                    ToRoom = "level Thirty room"
                }
            };

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Thirty")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Thirty target")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Thirty room")), Times.Once);


        }
        [Fact]
        public void Should_show_level_based_message_for_level_Forty()
        {

            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;
            _player.Level = 35;
            _Spells.LevelBasedMessages = new LevelBasedMessages()
            {
                HasLevelBasedMessages = true,
                Forty = new Messages()
                {
                    ToPlayer = "level forty",
                    ToTarget = "level forty target",
                    ToRoom = "level forty room"
                }
            };

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level forty")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level forty target")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level forty room")), Times.Once);


        }

        [Fact]
        public void Should_show_level_based_message_for_level_Fifty()
        {

            var writer = new Mock<IWriteToClient>();
            var mock = new Mock<Spells>(writer.Object)
            {
                CallBase = true
            };

            var spell = mock.Object;
            _player.Level = 45;
            _Spells.LevelBasedMessages = new LevelBasedMessages()
            {
                HasLevelBasedMessages = true,
                Fifty = new Messages()
                {
                    ToPlayer = "level Fifty",
                    ToTarget = "level Fifty target",
                    ToRoom = "level Fifty room"
                }
            };

            spell.DoSpell(_Spells, _player, _target, _room);

            mock.Verify(m => m.DoSpell(_Spells, _player, _target, _room), Times.Exactly(1));
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Fifty")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Fifty target")), Times.Once);
            writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "level Fifty room")), Times.Once);


        }
    }
}
