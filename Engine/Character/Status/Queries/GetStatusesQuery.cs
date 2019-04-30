using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Class.Queries
{
    public class GetStatusesQuery
    {
        public List<Option> GetStatuses()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Option>("Status");

                var data = col.FindAll().OrderBy(x => x.Name).ToList();

                return data;
            }
        }
    }
}
