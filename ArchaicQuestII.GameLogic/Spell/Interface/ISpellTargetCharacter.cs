using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Spell.Interface
{
    public interface ISpellTargetCharacter
    {

        public Player GetTarget(string target, Room room);
        public Player CheckTarget(Skill.Model.Skill spell, string target, Room room, Player player);
        public Player ReturnTarget(Skill.Model.Skill spell, string target, Room room, Player player);
    }
}
