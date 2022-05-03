using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Room;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Commands.Debug
{
    public class Debug : IDebug
    {
        private readonly IWriteToClient _writeToClient;

        public Debug(IWriteToClient writeToClient)
        {
            _writeToClient = writeToClient;
        }

        public void DebugRoom(Room room, Player player)
        {

            var jsonObject = JsonConvert.SerializeObject(room);

            _writeToClient.WriteLine(jsonObject, player.ConnectionId);
        }
    }
}
