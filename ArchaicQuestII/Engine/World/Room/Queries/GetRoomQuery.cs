using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.World.Room.Queries
{
    public class GetRoomQuery
    {
        public Model.Room GetRoom(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {

                var roomCol = db.GetCollection<Model.Room>("Room");

                var room = roomCol.FindById(id);

                return room;
            }
        }
    }
}
