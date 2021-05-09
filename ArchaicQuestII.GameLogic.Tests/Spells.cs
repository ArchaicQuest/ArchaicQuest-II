using System;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Skill.Enum;
using ArchaicQuestII.GameLogic.Skill.Model;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Model;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using Xunit;


namespace ArchaicQuestII.GameLogic.Tests
{
    public class SpellTests
    {
        private readonly Spell.Model.Spell _Spells;
        private readonly Player _player;
        private readonly Player _target;
        private readonly Room _room;
        private readonly CastSpell _spell;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IDamage> _damage;
        private readonly Mock<ICache> _cache;
        private readonly Mock<ISpellTargetCharacter> _spellTargetCharacter;
        private readonly Mock<IUpdateClientUI> _updateClientUI;
        private readonly Mock<IMobScripts> _mobScript;
        private readonly Mock<IDice> _dice;
        private readonly Mock<IDamageSpells> _damageSpells;

        public SpellTests()
        {

  


            _Spells = new Spell.Model.Spell()
            {
                Name = "Ogre strength",
                Effect = new Effect.Effect()
                {
                    Modifier = new EffectModifier()
                    {
                        Value = 10,
                        PositiveEffect = true
                    },
                    Location = EffectLocation.Strength,
                    Name = "OgreStrength",
                    Duration = new EffectModifier()
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
                    DiceMinSize = 10,
                    DiceMaxSize = 10
                },
                Description = "Makes you strong as an ogre",
                Rounds = 1,
                SpellGroup = new Sphere()

            };
            _player = new Player()
            {
                Name = "Malleus",
                ClassName = "Paladin",
                Status = CharacterStatus.Status.Standing,
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>
                    {
                        {EffectLocation.Mana, 250},
                        {EffectLocation.Strength, 0}
                    }
                },
                Spells = new List<Spell.Model.Spell>(),
                Skills = new List<SkillList>()
            };

            _player.Spells.Add(new Spell.Model.Spell()
            {
                Name = "chill touch",
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>
                    {

                {Cost.Mana, 10}

                    }
                }

            });
            _target = new Player()
            {
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>
                   {
                       { EffectLocation.Strength, 0 },
                       {EffectLocation.Hitpoints, 2500},
                   }
                }

            };
            _room = new Room();
            _writer = new Mock<IWriteToClient>();
            _damage = new Mock<IDamage>();
            _spellTargetCharacter = new Mock<ISpellTargetCharacter>();
            _cache = new Mock<ICache>();
            _updateClientUI = new Mock<IUpdateClientUI>();
            _mobScript = new Mock<IMobScripts>();
            _dice = new Mock<IDice>();
            _damageSpells = new Mock<IDamageSpells>();
            _spell = new CastSpell(_writer.Object, _spellTargetCharacter.Object, _cache.Object, _damage.Object, _updateClientUI.Object, _mobScript.Object,_dice.Object, _damageSpells.Object);

            var newSkill = new Skill.Model.Skill
            {
                Name = "magic missile",
                Id = 1,

            };

           
 
        }

        [Fact]
        public void Cant_cast_while_sleeping()
        {

            _player.Status = CharacterStatus.Status.Sleeping;

            Assert.False(_spell.ValidStatus(_player), "You can't do this while asleep.");
        }

        [Fact]
        public void Cant_cast_while_standing()
        {

            _player.Status = CharacterStatus.Status.Standing;

            Assert.True(_spell.ValidStatus(_player));
        }

        [Fact]
        public void Cant_cast_while_fighting()
        {

            _player.Status = CharacterStatus.Status.Fighting;

            Assert.True(_spell.ValidStatus(_player));
        }


        [Fact]
        public void Cant_cast_while_stunned()
        {

            _player.Status = CharacterStatus.Status.Stunned;

            Assert.False(_spell.ValidStatus(_player), "You are stunned.");
        }

        [Fact]
        public void Cant_cast_while_busy()
        {

            _player.Status = CharacterStatus.Status.Busy;

            Assert.False(_spell.ValidStatus(_player), "You can't do that right now.");
        }

        [Fact]
        public void Cant_cast_while_dead()
        {

            _player.Status = CharacterStatus.Status.Dead;

            Assert.False(_spell.ValidStatus(_player), "You can't do this while dead.");
        }

        [Fact]
        public void Cant_cast_while_ghost()
        {

            _player.Status = CharacterStatus.Status.Ghost;

            Assert.False(_spell.ValidStatus(_player), "You can't do this while dead.");
        }

        [Fact]
        public void Cant_cast_while_Incapacitated()
        {

            _player.Status = CharacterStatus.Status.Incapacitated;

            Assert.False(_spell.ValidStatus(_player), "You can't do this while dead.");
        }

        [Fact]
        public void Cant_cast_while_sitting()
        {

            _player.Status = CharacterStatus.Status.Sitting;

            Assert.False(_spell.ValidStatus(_player), "You need to stand up before you do that.");
        }

        [Fact]
        public void Cant_cast_while_resting()
        {

            _player.Status = CharacterStatus.Status.Resting;

            Assert.False(_spell.ValidStatus(_player), "You need to stand up before you do that.");
        }


        [Fact]
        public void Cant_find_spell()
        {

            _player.Status = CharacterStatus.Status.Standing;
            var foundSpell = _spell.FindSpell("magic", _player);

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "You don't know a spell that begins with magic")), Times.Once);
            Assert.True(foundSpell == null);
        }

        //[Fact]
        //public void Can_find_spell()
        //{

        //    _player.Status = CharacterStatus.Status.Standing;
        //    _player.Skills.Add(new SkillList() {
        //        SkillName = "magic missile",
        //        Level = 1,
        //        SkillId = 1
        //        });
        //    var foundSpell = _spell.FindSpell("magic missile", _player);

        //    Assert.True(foundSpell != null);
        //}

        [Fact]
        public void Not_enough_mana()
        {
            _player.Level = 45;
            _player.Attributes = new Attributes()
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    {EffectLocation.Mana, 0}
                }
            };

            _Spells.Cost.Table[Cost.Mana] = 100;

            var spell = new Spell.Model.Spell()
            {
                Name = "chill touch",
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>
                    {

                        {Cost.Mana, 100}

                    }
                }

            };

           
            Assert.False(_spell.ManaCheck(spell, _player));
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "You don't have enough mana."), "mob"), Times.Once);

        }

        [Fact]
        public void Has_enough_mana()
        {
            _player.Level = 45;
            _player.Attributes = new Attributes()
            {
                Attribute = new Dictionary<EffectLocation, int>
                {
                    {EffectLocation.Mana, 100}
                }
            };

          

            var spell = new Spell.Model.Spell()
            {
                Name = "chill touch",
                Cost = new SkillCost()
                {
                    Table = new Dictionary<Cost, int>
                    {

                        {Cost.Mana, 10}

                    }
                }

            };


            Assert.True(_spell.ManaCheck(spell, _player));
           
        }


        [Fact]
        public void Does_spell_affect_character()
        {

            
            _player.Status = CharacterStatus.Status.Standing;
           var spell = new Spell.Model.Spell()
            {
                Name = "Magic missile",
                ValidTargets =  ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim
            };
           
            Assert.True(_spell.SpellAffectsCharacter(spell));
        }


        [Fact]
        public void message_for_cast_on_self()
        {

            var spell = new Spell.Model.Spell()
            {
                Name = "Magic missile",
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim
            };

            var room = new Room();

            _player.Status = CharacterStatus.Status.Standing;
            _player.Gender = "Male";
            _target.Id = Guid.NewGuid();
            _player.Id = _target.Id;
            _player.ConnectionId = _target.Id.ToString();
            _player.ClassName = "Mage";
            _player.Attributes.Attribute[EffectLocation.Mana] = 500;

            _spell.ReciteSpellCharacter(_player, _target, spell, room);
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "Malleus closes his eyes and utters the words, 'Magic missile'."), _player.ConnectionId), Times.Once);

        }

        [Fact]
        public void message_for_cast_on_not_self()
        {
            var room = new Room();
            var spell = new Spell.Model.Spell()
            {
                Name = "Magic missile",
                ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim
            };

            _player.Status = CharacterStatus.Status.Standing;
            _player.Gender = "Male";
            _target.Name = "Bob";
            _target.ClassName = "Mage";
            _target.Id = Guid.NewGuid();
            _player.Id = Guid.NewGuid();
            _player.ConnectionId = "abc";
            _target.ConnectionId = "bcd";

            _spell.ReciteSpellCharacter(_player, _target, spell, room);
            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "You look at Bob and utters the words, 'Magic missile'."), _player.ConnectionId), Times.Once);

            _writer.Verify(w => w.WriteLine(It.Is<string>(s => s == "Malleus looks at you and utters the words, 'Magic missile'."), _target.ConnectionId), Times.Once);

        }


        [Fact]
        public void Spell_Name()
        {
            

           var spellName =  _spell.ObsfucateSpellName("ice storm");
            
           Assert.Equal("uqz ghafw", spellName);

        }


        //[Fact]
        //public void Should_cast_spell()
        //{

        //    var spell = new Spell.Model.Spell()
        //    {
        //        Name = "magic",
        //        Cost = new SkillCost()
        //        {
        //            Table = new Dictionary<Cost, int>()
        //            {
        //                {Cost.Mana, 5}
        //            }
        //        },
        //        Damage = new Dice()
        //        {
        //            DiceMaxSize = 8,
        //            DiceMinSize = 1,
        //            DiceRoll = 4
        //        },
        //        Rounds = 1,
        //        StartsCombat = true,
        //        Type = SkillType.None,
        //        ValidTargets = ValidTargets.TargetPlayerRoom | ValidTargets.TargetFightVictim
        //    };

        //    _player.Status = CharacterStatus.Status.Standing;
        //    _player.Gender = "Male";
        //    _player.Spells = new List<Spell.Model.Spell>()
        //    {
        //        spell
        //    };

        //    _target.Name = "Bob";

        //    _target.Id = Guid.NewGuid();
        //    _player.Id = Guid.NewGuid();

        //    _room.Players.Add(_player);
        //    _room.Players.Add(_target);

        //    _spellTargetCharacter.Setup(r => r.ReturnTarget(spell, "Bob", _room, _player)).Returns(_target);

        //    _spell.DoSpell("Fireball", _player, "Bob", _room);
        //    Assert.True(_target.Attributes.Attribute[EffectLocation.Hitpoints] < 2500);
        //    //_writer.Verify(w => w.WriteLine(It.Is<string>(s => s.StartsWith("Your Fireball"))), Times.Once);

        //}





    }
}
