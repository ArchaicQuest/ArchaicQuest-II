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
        
        /// <summary>
        /// Returns a string of player difficulty for area 
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        string AreaConsider(Player player, Room.Room room);
        
        /// <summary>
        /// Display player population in area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        string AreaPopulation(Player player, Room.Room room);
        
        string GetAreaLevelScale(Area area);
    }
}