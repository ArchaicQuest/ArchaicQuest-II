using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class SkillLogic
    {
        private readonly IWriteToClient _writer;
        private readonly IUpdateClientUI _updateClientUi;
        private readonly IDamage _damage;
        private readonly ICombat _fight;
        private readonly ISkillManager _skillManager;

        public SkillLogic(
            IWriteToClient writer, 
            IUpdateClientUI updateClientUi,
            IDamage damage, 
            ICombat fight, 
            ISkillManager skillManager)
        {
            _writer = writer;
            _updateClientUi = updateClientUi;
            _damage = damage;
            _fight = fight;
            _skillManager = skillManager;

        }

        public int Kick(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var damage = DiceBag.Roll(1, 1, 8) + player.Attributes.Attribute[EffectLocation.Strength] / 6;

            if (target == null)
            {

            }

            _skillManager.DamagePlayer("Kick", damage, player, target, room);

            return damage;
        }
    }
}
