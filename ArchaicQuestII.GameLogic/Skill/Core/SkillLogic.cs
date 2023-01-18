using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Effect;
using ArchaicQuestII.GameLogic.Utilities;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Core
{
    public class SkillLogic
    {
        private readonly ICharacterHandler _characterHandler;

        public SkillLogic(
            ICharacterHandler characterHandler)
        {
            _characterHandler = characterHandler;
        }

        public int Kick(Player player, Player target, Room room)
        {
            var casterLevel = player.Level > 10 ? 5 : player.Level;
            var damage = DiceBag.Roll(1, 1, 8) + player.Attributes.Attribute[EffectLocation.Strength] / 6;

            if (target == null)
            {
                return 0;
            }

            _characterHandler.DamagePlayer("Kick", damage, player, target, room);

            return damage;
        }
    }
}
