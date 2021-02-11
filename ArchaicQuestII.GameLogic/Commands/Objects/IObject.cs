using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Commands.Objects
{

    /// <summary>
    /// How one can interact with objects
    /// </summary>
   public interface IObject
    {
        void Get(string target, string container, Room room, Player player, string? fullCommand);
        void Give(string itemName, string targetName, Room room, Player player, string command);
        void GetAll(Room room, Player player);
        void Drop(string target, string container, Room room, Player player, string command);
        bool DropGold(string command, Room room, Player player);
        bool GiveGold(string command, Room room, Player player);
        void Open(string target, Room room, Player player);
        void Close(string target, Room room, Player player);
        void Delete(string target, Room room, Player player);
        void Unlock(string target, Room room, Player player);
        void Lock(string target, Room room, Player player);
    }
}
