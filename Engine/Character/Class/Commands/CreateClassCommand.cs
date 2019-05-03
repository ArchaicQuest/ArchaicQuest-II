using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class CreateClassCommand
    {
        public void CreateClass(Model.Class charClass)
        {
          
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Model.Class>("Class");

                    col.Insert(charClass);
                    col.EnsureIndex(x => x.Name);
                }
        }
    }
}
