using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;

namespace ArchaicQuestII.GameLogic.World.Area
{
    /// <summary>
    /// Handles all actions relating to areas
    /// </summary>
    public class AreaActions : IAreaActions
    {
        private readonly IWriteToClient _writeToClient;
        private readonly IDataBase _db;
        
        public AreaActions(IWriteToClient writeToClient, IDataBase db)
        {
            _writeToClient = writeToClient;
            _db = db;
        }

        /// <summary>
        /// Display notice when player enters a new area
        /// </summary>
        /// <param name="player">Player entering area</param>
        /// <param name="room">Room that was entered</param>
        public void AreaEntered(Player player, Room.Room room)
        {
            var area = _db.GetCollection<Area>(DataBase.Collections.Area).FindById(room.AreaId);

            _writeToClient.WriteLine($"<p>You have traversed into <b>{area.Title}</b>.", player.ConnectionId);
        }
    }
}