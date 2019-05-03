using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class SeedAttackTypesCommand
    {


        public void Seed()
        {

            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                if (db.CollectionExists("AttackType"))
                {
                    return;
                }

                var command = new CreateAttackTypeCommand();

                foreach (var data in SeedData())
                {
                    command.CreateAttackType(data);
                }



            }

        }

        public List<Option> SeedData()
        {
            var seedData = new List<Option>()
            {
              new Option()
              {
                  Name = "Punch",
                  CreatedBy = "Malleus"
              },
                new Option()
                {
                    Name = "Pound",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Bite",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Charge",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Peck",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Headbutt",
                    CreatedBy = "Malleus"
                },
            };

            return seedData;
        }
    }
}
