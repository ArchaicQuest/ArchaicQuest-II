using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
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
        public void AreaConsider(Player player, Room.Room room)
        {
            var mobLevels = 0;
            var mobCount = 0;
            
            foreach (var mob in _cache.GetAllRoomsInArea(room.AreaId).SelectMany(r => r.Mobs))
            {
                mobLevels += mob.Level;
                mobCount++;
            }

            var dangerLevel = mobCount == 0 ? 0 : mobLevels/mobCount - player.Level;

            switch (dangerLevel)
            {
                case > 10: 
                    _writeToClient.WriteLine("{red}You feel nervous here!{/}", player.ConnectionId);
                    break;
                case > 5:
                    _writeToClient.WriteLine("{yellow}You feel anxious here.{/}", player.ConnectionId);
                    break;
                case > 1:
                    _writeToClient.WriteLine("{blue}You feel comfortable here.{/}", player.ConnectionId);
                    break;
                default:
                    _writeToClient.WriteLine("{green}You feel relaxed here.{/}", player.ConnectionId);
                    break;
            }
        }
        
        /// <summary>
        /// Display player population in area
        /// </summary>
        /// <param name="player">Player entering command</param>
        /// <param name="room">Room where command was entered</param>
        public void AreaPopulation(Player player, Room.Room room)
        {
            var playerCount = _cache.GetAllRoomsInArea(room.AreaId).SelectMany(r => r.Players).Count();
            
            switch (playerCount)
            {
                case > 30: 
                    _writeToClient.WriteLine("The area shows signs of being heavily traveled.", player.ConnectionId);
                    break;
                case > 20:
                    _writeToClient.WriteLine("The area shows signs of being well traveled.", player.ConnectionId);
                    break;
                case > 10:
                    _writeToClient.WriteLine("The area shows signs of being traveled.", player.ConnectionId);
                    break;
                case > 1:
                    _writeToClient.WriteLine("The area shows signs of being lightly traveled.", player.ConnectionId);
                    break;
                default:
                    _writeToClient.WriteLine("The area shows no signs of being traveled.", player.ConnectionId);
                    break;
            }
        }
        
        /// <summary>
        /// Display info about all areas
        /// </summary>
        /// <param name="player">Player entering command</param>
        public void AreaList(Player player)
        {
            var sb = new StringBuilder();
            var areas = _db.GetCollection<Area>(DataBase.Collections.Area).FindAll().ToList();
         
            foreach (var area in areas)
            {
                area.Rooms = _cache.GetAllRoomsInArea(area.Id);
            }
       

            sb.Append($"Total Areas: {areas.Count}");
            sb.Append("<ul>");
           
            foreach (var area in areas)
            {
                sb.Append($"<li>[{GetAreaLevelScale(area)}] {area.Title}");
                if (area.CreatedBy != null)
                    sb.Append($" ({area.CreatedBy})");
                sb.Append("</li>");
            }

            sb.Append("</ul>");
            
            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        #region Helpers

        /// <summary>
        /// Helper to get area levels
        /// </summary>
        /// <param name="area">Area to get level scale</param>
        private string GetAreaLevelScale(Area area)
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
        
        #endregion
    }
}