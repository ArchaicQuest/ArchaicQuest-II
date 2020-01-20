using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Queries
{
    public class GetAlignmentsQuery
    {
        public List<Alignment> GetAlignments()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Alignment>("Alignment");

                var data = col.FindAll().OrderBy(x => x.Name).ToList();

                return data;
            }
        }
    }
}
