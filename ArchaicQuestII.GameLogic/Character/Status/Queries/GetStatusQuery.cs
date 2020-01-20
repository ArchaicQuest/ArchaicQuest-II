using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Queries
{
    public class GetStatusQuery
    {
        public Option GetStatus(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Option>("Status");

                var data = col.FindById(id);

                return data;
            }
        }
    }
}
