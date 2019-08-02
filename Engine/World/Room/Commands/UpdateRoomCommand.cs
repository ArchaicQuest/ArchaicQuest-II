using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.World.Room.Commands
{
    public class UpdateRoomCommand
    {

        public void UpdateRoom(Model.Room room)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Room>("Room");
                var data = col.FindById(room.Id);

                if (data == null)
                {
                    return;
                }
              
                col.Upsert(room);


            }

        }
    }
}
