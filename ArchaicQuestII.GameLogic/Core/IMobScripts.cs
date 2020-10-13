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
       public void Say(string n, int delay, Room room, Player player, Player mob);
        public string GetName(Player player);
       public void UpdateInv(Player player);
       public void AttackPlayer(Room room, Player player, Player mob);
       public void AddEventState(Player player, string key, int value);
       public void UpdateEventState(Player player, string key, int value);
        public int ReadEventState(Player player, string key);
       public bool HasEventState(Player player, string key);
       public int GetPlayerAttribute(Player player, string attribute);

       public int Random(int min, int max);
       public string GetClass(Player player);
       public bool IsPC(Player player);
       public bool IsMob(Player player);
       public bool IsGood(Player player);
       public bool IsEvil(Player player);
       public bool IsNeut(Player player);
       public bool IsMobHere(string name, Room room);
       public bool IsObjectHere(string name, Room room);
       public bool IsImm(Player player);
       public bool HasObject(Player player, string name);
    }
}
