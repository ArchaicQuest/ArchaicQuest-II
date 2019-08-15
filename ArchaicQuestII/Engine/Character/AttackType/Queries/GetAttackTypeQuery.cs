using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Queries
{
    public class GetAttackTypeQuery
    {
        public OptionDescriptive GetAttackType(int id)
        {
            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                var col = db.GetCollection<OptionDescriptive>("AttackType");

                var data = col.FindById(id);

                return data;
            }
        }
    }
}
