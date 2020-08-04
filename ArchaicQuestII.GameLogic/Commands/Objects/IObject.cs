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
        void Get(string target, Room room, Player player);
        void Drop(string target, Room room, Player player);
        void Open(string target, Room room, Player player);
        void Close(string target, Room room, Player player);
        void Delete(string target, Room room, Player player);
    }
}
