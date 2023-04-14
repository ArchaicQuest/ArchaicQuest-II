using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Area
{
    /// <summary>
    /// Handles all actions relating to areas
    /// </summary>
    public static class AreaActions
    {
        /// <summary>
        /// Display notice when player enters a new area
        /// </summary>
        /// <param name="player">Player entering area</param>
        /// <param name="room">Room that was entered</param>
        public static void AreaEntered(Player player, Room.Room room)
        {
            var area = Services.Instance.DataBase
                .GetCollection<Area>(DataBase.Collections.Area)
                .FindById(room.AreaId);

            Services.Instance.Writer.WriteLine(
                $"<p>You have traversed into <b>{area.Title}</b>.",
                player
            );
        }
    }
}
