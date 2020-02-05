using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Interface
{
    public interface ISpells
    {
        void DoSpell(Model.Spell spell, Player origin, Player target, Room room = null);
    }
}

