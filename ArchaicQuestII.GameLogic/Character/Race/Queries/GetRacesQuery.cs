using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Events;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Race.Commands
{
    public class GetRacesQuery
    {
        public List<Model.Race> GetRaces()
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Race>("Race");

                var race = col.FindAll().ToList();

                return race;
            }
        }
    }
}
