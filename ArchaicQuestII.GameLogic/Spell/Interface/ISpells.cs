using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Interface
{
    public interface ISpells
    {
        void DoSpell(string spellName, Player origin, string targetName, Room room = null);
        void DamagePlayer(string spellName, int damage, Player player, Player target, Room room);
    }
}

