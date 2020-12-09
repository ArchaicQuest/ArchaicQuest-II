using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Core
{
   public class Core: ICore
    {
        private readonly ICache _cache;
        private readonly IWriteToClient _writeToClient;
        private readonly IDataBase _db;
        public Core(ICache cache, IWriteToClient writeToClient, IDataBase db)
        {
            _cache = cache;
            _writeToClient = writeToClient;
            _db = db;
        }
        public void Who(Player player)
        {
           
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var pc in _cache.GetPlayerCache())
            {
                sb.Append(
                    $"<li>[{pc.Value.Level} {pc.Value.Race} {pc.Value.ClassName}] ");
                sb.Append(
                    $"<span class='player'>{pc.Value.Name}</span></li>");
            }

            sb.Append("</ul>");
            sb.Append($"<p>Players found: {_cache.GetPlayerCache().Count}</p>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);


        }

        public void Where(Player player, Room room)
        {
            var area = _cache.GetAllRoomsInArea(room.AreaId);
            var areaName = _db.GetCollection<Area>(DataBase.Collections.Area).FindById(room.AreaId);

            var sb = new StringBuilder();

            sb.Append($"<p>{areaName.Title}</p><p>Players near you:</p>");
            sb.Append("<ul>");
            foreach (var rm in area)
            {
                foreach (var pc in rm.Players)
                {
                    sb.Append(
                        $"<li>{pc.Name} - {rm.Title}");
                }
                
            }

            sb.Append("</ul>");

            _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
        }

        public void QuestLog(Player player)
        {
           var sb = new StringBuilder();

           foreach (var q in player.QuestLog)
           {
               sb.Append($"<div class='quest-block'><h3>{q.Title}</h3><p>{q.Area}</p><p>Kill:</p><ol>");

               foreach (var mob in q.MobsToKill)
               {
                   sb.Append($"<li>{mob.Name} {mob.Current}/{mob.Count}</li>");
               }

               sb.Append($"</ol><p>{q.Description}</p><p>Reward:</p><ul><li>{q.ExpGain} Experience points</li><li>{q.GoldGain} Gold</li>");

               foreach (var i in q.ItemGain)
               {
                   sb.Append($"<li>{i.Name}</li>");
               }

               sb.Append("</ul></div>");
           }

           _writeToClient.WriteLine(sb.ToString(), player.ConnectionId);
                         
        }
    }
}
