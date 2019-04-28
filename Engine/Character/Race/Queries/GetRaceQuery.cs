using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Events;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Race.Commands
{
    public class GetRaceQuery
    {
        public Model.Race GetRace(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<Model.Race>("Race");

                var race = col.FindById(id);

                return race;
            }
        }
    }
}
