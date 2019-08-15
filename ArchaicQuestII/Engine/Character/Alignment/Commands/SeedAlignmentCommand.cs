using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class SeedAlignmentCommand
    {


        public void Seed()
        {

            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                if (db.CollectionExists("Alignment"))
                {
                    return;
                }

                var command = new CreateAlignmentCommand();

                foreach (var data in SeedData())
                {
                    command.CreateAlignment(data);
                }



            }

        }

        public List<Alignment> SeedData()
        {
            var seedData = new List<Alignment>()
            {
              new Alignment()
              {
                  Name = "Pure and Holy",
                  Value = 1000,
                  CreatedBy = "Malleus"
              },
                new Alignment()
                {
                    Name = "Extremely Good",
                    Value = 900,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Very Good",
                    Value = 350,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Good",
                    Value = 100,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Neutral leaning towards good",
                    Value = -100,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Neutral",
                    Value = -350,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Neutral leaning towards evil",
                    Value = -600,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Evil",
                    Value = -900,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Very evil",
                    Value = -1000,
                    CreatedBy = "Malleus"
                },
                new Alignment()
                {
                    Name = "Pure evil",
                    Value = -1000,
                    CreatedBy = "Malleus"
                },
            };

            return seedData;
        }
    }
}
