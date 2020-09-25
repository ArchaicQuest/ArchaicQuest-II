using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
   public interface IMobScripts
   {
       public bool IsInRoom(Room room, Player player);
       public void Say(string n, int delay, Room room, Player player);
       public string GetName(Player player);
       public void UpdateInv(Player player);
       public void AttackPlayer(Room room, Player player, Player mob);
       public void AddEventState(Player player, string key, int value);
       public void UpdateEventState(Player player, string key, int value);
        public int ReadEventState(Player player, string key);
       public bool HasEventState(Player player, string key);
   }
}
