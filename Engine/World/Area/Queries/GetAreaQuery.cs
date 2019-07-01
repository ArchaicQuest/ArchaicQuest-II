using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.World.Area.Queries
{
    public class GetAreaQuery
    {
        public Model.Area GetArea(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Area>("Area");

                var data = col.FindById(id);

                var roomCol = db.GetCollection<Room.Room>("Room");

                var rooms = roomCol.Find(x => x.AreaId == id);

                data.Rooms = rooms.ToList();

                return data;
            }
        }
    }
}
