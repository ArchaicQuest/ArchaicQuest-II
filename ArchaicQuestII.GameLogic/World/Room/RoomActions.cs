using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class RoomActions
    {

        private readonly IWriteToClient _writeToClient;
        public RoomActions(IWriteToClient writeToClient)
        {
            _writeToClient = writeToClient;
        }
        /// <summary>
        /// Displays current room 
        /// </summary>
        public void Look(Room room, Player player)
        {
            var roomDesc = new StringBuilder();

            roomDesc.Append($"<h5>{room.Title}</h5>")
                .Append($"<p>{room.Description}</p>");
           _writeToClient.WriteLine(roomDesc.ToString(), player.ConnectionId);
        }
    }
}
