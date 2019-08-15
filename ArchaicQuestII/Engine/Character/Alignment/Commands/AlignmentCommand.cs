using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class CreateAlignmentCommand
    {
        public void CreateAlignment(Alignment option)
        {
          
                using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
                {
                    var col = db.GetCollection<Option>("Alignment");

                    col.Insert(option);
                    col.EnsureIndex(x => x.Name);
                }
        }
    }
}
