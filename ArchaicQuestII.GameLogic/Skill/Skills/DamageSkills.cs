using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Skills
{

    public interface IDamageSkills
    {
        int Kick(Player player, Player target, Room room);
        int Elbow(Player player, Player target, Room room);

    }

    public class DamageSkills : IDamageSkills
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDice _dice;
        private readonly IDamage _damage;
        private readonly ICombat _fight;
        private readonly ISkillManager _skillManager;



        public DamageSkills(IWriteToClient writer, IUpdateClientUI updateClientUi, IDice dice, IDamage damage, ICombat fight, ISkillManager skillManager)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _dice = dice;
            _damage = damage;
            _fight = fight;
            _skillManager = skillManager;

        }

        public int Kick(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 8) + str / 4;
 
            _skillManager.DamagePlayer("Kick", damage, player, target, room);

            player.Lag += 1;


            _skillManager.updateCombat(player, target);

            return damage;
        }

        public int Elbow(Player player, Player target, Room room)
        {
            var str = player.Attributes.Attribute[EffectLocation.Strength];
            var damage = _dice.Roll(1, 1, 4) + str / 5;

            _skillManager.DamagePlayer("elbow", damage, player, target, room);

            player.Lag += 1;


            _skillManager.updateCombat(player, target);

            return damage;
        }
 
    }
}
