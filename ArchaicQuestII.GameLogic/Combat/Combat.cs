using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Combat
{
   public class Combat
    {
        public Player findTarget(Player target, Room room)
        {
            // If mob
            if (target?.PlayTime == 0)
            {
                return (Player)room.Mobs.FirstOrDefault(x => x.Name.Equals(target.Name));
            }

            return (Player)room.Players.FirstOrDefault(x => x.Name.Equals(target.Name));
        }
    }
}
