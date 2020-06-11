using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Commands.Debug
{
    public interface IDebug
    {
        /// <summary>
        /// Returns the Room data for inspection
        /// </summary>
        /// <param name="room">The room object</param>
        /// <param name="player"></param>
        void DebugRoom(Room room, Player player);
    }
}
