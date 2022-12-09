using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.World.Area
{
    /// <summary>
    /// Handles all actions relating to areas 
    /// </summary>
    public interface IAreaActions
    {
        /// <summary>
        /// Display basic information about area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        void AreaInfo(Player player, Room.Room room);
        
        /// <summary>
        /// Display notice when player enters a new area
        /// </summary>
        /// <param name="player">Player entering area</param>
        /// <param name="room">Room that was entered</param>
        void AreaEntered(Player player, Room.Room room);
        
        /// <summary>
        /// Display player difficulty for area 
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        void AreaConsider(Player player, Room.Room room);
        
        /// <summary>
        /// Display player population in area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        void AreaPopulation(Player player, Room.Room room);
        
        /// <summary>
        /// Display level info about all areas
        /// </summary>
        /// <param name="player">Player entering command</param>
        void AreaList(Player player);
    }
}