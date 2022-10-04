using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
    public interface IMobScripts
    {
        public bool IsInRoom(Room room, Player player);
        [Obsolete("Say is deprecated, please use MobSay or MobEmote instead.")]
        public void Say(string n, int delay, Room room, Player player);
        public void MobSay(string n, Room room, Player player, Player mob, int delay);
        public void MobEmote(string n, Room room, Player player, int delay);
        [Obsolete("Say is deprecated, please use MobSay or MobEmote instead.")]
        public void Say(string n, int delay, Room room, Player player, Player mob);
        public string GetName(Player player);
        public bool Contains(string word, string expected);
        public bool StartsWith(string word, string expected);
        public void GainXP(Player player, int xp);
        public void UpdateInv(Player player);
        public void AttackPlayer(Room room, Player player, Player mob);
        public void AddEventState(Player player, string key, int value);
        public void UpdateEventState(Player player, string key, int value);
        public int ReadEventState(Player player, string key);
        public bool HasEventState(Player player, string key);
        public int GetPlayerAttribute(Player player, string attribute);
        public void GiveGold(int value, Player player);

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
        public void AddQuest(Player player, int id);
        void CompleteQuest(Player player, int questId);
        void DoSkill(Player player, Player mob, Room room);
        Task Sleep(int milliseconds);
        void RemoveMobFromRoom(Player mob, Room room);
        public void Follow(Player player, Player mob);
        public void UnFollow(Player player, Player mob);
        public bool CanFollow(Player player);
    }
}
