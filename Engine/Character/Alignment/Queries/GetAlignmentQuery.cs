using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Class.Queries
{
    public class GetAlignmentQuery
    {
        public Alignment GetAlignment(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Alignment>("Alignment");

                var data = col.FindById(id);

                return data;
            }
        }
    }
}
