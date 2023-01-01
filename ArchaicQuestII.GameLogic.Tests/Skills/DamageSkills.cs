using System;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;
using Moq;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Class;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Skills;


namespace ArchaicQuestII.GameLogic.Tests.Skills
{
    public class DamageSkillsTests
    {
        private readonly Spell.Model.Spell _Spells;
        private readonly Player _player;
        private readonly Player _target;
        private readonly Room _room;
        private readonly CastSpell _spell;
        private readonly Mock<IWriteToClient> _writer;
        private readonly Mock<IDamage> _damage;
        private readonly Mock<ICombat> _combat;
        private readonly Mock<ICache> _cache;
        private readonly Mock<ISkillManager> _skillManager;
        private readonly Mock<IUpdateClientUI> _updateClientUI;
        private readonly Mock<IMobScripts> _mobScript;
        private readonly DamageSkills _damageSkills;

        public DamageSkillsTests()
        {


            _player = new Player()
            {
                Id = Guid.NewGuid(),
                Name = "Malleus",
                ClassName = "Paladin",
                Status = CharacterStatus.Status.Standing,
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>
                    {
                        {EffectLocation.Mana, 250},
                        {EffectLocation.Hitpoints, 10000},
                        {EffectLocation.Moves, 10000},
                        {EffectLocation.Strength, 60},
                        {EffectLocation.Dexterity, 60},
                        {EffectLocation.Constitution, 60},
                        {EffectLocation.Intelligence, 60},
                        {EffectLocation.Wisdom, 60},
                        {EffectLocation.Charisma, 60},
                    }
                },
                Spells = new List<Spell.Model.Spell>(),
                Skills = new List<SkillList>()
            };


            _target = new Player()
            {
                Id = Guid.NewGuid(),
                Name = "Gary",
                Status = CharacterStatus.Status.Standing,
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>
                   {
                       {EffectLocation.Mana, 250},
                       {EffectLocation.Hitpoints, 10000},
                       {EffectLocation.Moves, 10000},
                       {EffectLocation.Strength, 60},
                       {EffectLocation.Dexterity, 60},
                       {EffectLocation.Constitution, 60},
                       {EffectLocation.Intelligence, 60},
                       {EffectLocation.Wisdom, 60},
                       {EffectLocation.Charisma, 60},
                   }
                }

            };
            _room = new Room();
            _writer = new Mock<IWriteToClient>();
            _damage = new Mock<IDamage>();
            _skillManager = new Mock<ISkillManager>();
            _cache = new Mock<ICache>();
            _combat = new Mock<ICombat>();
            _updateClientUI = new Mock<IUpdateClientUI>();

            _damageSkills = new DamageSkills(_writer.Object, _updateClientUI.Object, _damage.Object, _combat.Object, _skillManager.Object);
        }




    }
}
