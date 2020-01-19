using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.World.Area.Commands
{
    public class UpdateAreaCommand
    {

        public void UpdateArea(Model.Area area)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Area>("Area");
                var data = col.FindById(area.Id);

                data.Description = area.Description;
                data.DateUpdated = DateTime.Now;
                data.Title = area.Title;

                col.Upsert(data);


            }

        }
    }
}
