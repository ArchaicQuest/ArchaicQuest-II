using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell
{
    public interface ISpellList
    {
        void CastSpell(string key, string obj, Player target, string fullCommand, Player player, Room room, bool wearOff);
    }
}
