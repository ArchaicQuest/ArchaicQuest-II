using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Class.Queries
{
    public class GetClassQuery
    {
        public Model.Class GetClass(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Class>("Class");

                var charClass = col.FindById(id);

                return charClass;
            }
        }
    }
}
