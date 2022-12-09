using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.World.Area
{
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

        //Display information about area
        public void AreaInfo(Player player, Room.Room room)
        {
            var sb = new StringBuilder();
            var area = GetAreaFromRoom(room);
            var roomCount = _cache.GetAllRoomsInArea(room.AreaId).Count;

            sb.Append($"<p>You are currently in <b>{area.Title}</b>.</p><p>{area.Description}</p>");

            sb.Append(roomCount > 1
                ? $"<p>Area contains <b>{roomCount}</b> rooms.</p>"
                : "<p>Area contains <b>1</b> room.</p>");

            if (area.CreatedBy != null)
                sb.Append($"<p>(Created by {area.CreatedBy})</p>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void AreaEntered(Player player, Room.Room room)
        {
            var area = GetAreaFromRoom(room);

            _writeToClient.WriteLine($"<p>You have traversed into <b>{area.Title}</b>", player.ConnectionId);
        }

        //Display player difficulty for area 
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
        
        //Display player population in area
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

        //Helper to get area from room
        private Area GetAreaFromRoom(Room.Room room)
        {
            return _db.GetCollection<Area>(DataBase.Collections.Area).FindById(room.AreaId);
        }
    }
}