using System.Linq;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Area
{
    /// <summary>
    /// Handles all actions relating to areas
    /// </summary>
    public class AreaActions : IAreaActions
    {
        private readonly IWriteToClient _writeToClient;
        private readonly ICache _cache;
        private readonly IDataBase _db;
        
        public AreaActions(IWriteToClient writeToClient, ICache cache, IDataBase db)
        {
            _writeToClient = writeToClient;
            _cache = cache;
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
        
        /// <summary>
        /// Display player difficulty for area 
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        public string AreaConsider(Player player, Room.Room room)
        {
            var mobLevels = 0;
            var mobCount = 0;
            
            foreach (var mob in _cache.GetAllRoomsInArea(room.AreaId).SelectMany(r => r.Mobs))
            {
                mobLevels += mob.Level;
                mobCount++;
            }

            var dangerLevel = mobCount == 0 ? 0 : mobLevels/mobCount - player.Level;

            return dangerLevel switch
            {
                > 10 => "{red}You feel nervous here!{/}",
                > 5 => "{yellow}You feel anxious here.{/}",
                > 1 => "{blue}You feel comfortable here.{/}",
                _ => "{green}You feel relaxed here.{/}"
            };
        }
        
        /// <summary>
        /// Display player population in area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        public string AreaPopulation(Player player, Room.Room room)
        {
            var playerCount = _cache.GetAllRoomsInArea(room.AreaId).SelectMany(r => r.Players).Count();

            return playerCount switch
            {
                > 30 => "The area shows signs of being heavily traveled.",
                > 20 => "The area shows signs of being well traveled.",
                > 10 => "The area shows signs of being traveled.",
                > 1 => "The area shows signs of being lightly traveled.",
                _ => "The area shows no signs of being traveled."
            };
        }
        
        /// <summary>
        /// Helper to get area levels
        /// </summary>
        /// <param name="area">Area to get level scale</param>
        public string GetAreaLevelScale(Area area)
        {
            var minLvl = 999;
            var maxLvl = 0;
            var mobCount = 0;

            foreach (var mob in area.Rooms.Where(x => x.Mobs.Any()).SelectMany(room => room.Mobs))
            {
                if (mob.Level < minLvl)
                    minLvl = mob.Level;
                if (mob.Level > maxLvl)
                    maxLvl = mob.Level;

                mobCount++;
            }

            return mobCount == 0 ? "0 - 0" : $"{minLvl} - {maxLvl}";
        }
    }
}