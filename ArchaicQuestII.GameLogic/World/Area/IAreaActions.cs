using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Area
{
    /// <summary>
    /// Handles all actions relating to areas 
    /// </summary>
    public interface IAreaActions
    {
        /// <summary>
        /// Display notice when player enters a new area
        /// </summary>
        /// <param name="player">Player entering area</param>
        /// <param name="room">Room that was entered</param>
        void AreaEntered(Player player, Room.Room room);
    }
}