using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Class.Queries
{
    public class GetClassesQuery
    {
        public List<Model.Class> GetClasses()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Class>("Class");

                var classList = col.FindAll().ToList();

                return classList;
            }
        }
    }
}
