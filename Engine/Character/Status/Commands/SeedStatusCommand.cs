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
    public class SeedStatusCommand
    {


        public void Seed()
        {

            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                if (db.CollectionExists("Status"))
                {
                    return;
                }

                var command = new CreateStatusCommand();

                foreach (var data in SeedData())
                {
                    command.CreateStatus(data);
                }



            }

        }

        public List<Option> SeedData()
        {
            var seedData = new List<Option>()
            {
              new Option()
              {
                  Name = "Sitting",
                  CreatedBy = "Malleus"
              },
                new Option()
                {
                    Name = "Standing",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Sleeping",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Fighting",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Resting",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Incapitated",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Dead",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Ghost",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Busy",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Floating",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Mounted",
                    CreatedBy = "Malleus"
                },
                new Option()
                {
                    Name = "Stunned",
                    CreatedBy = "Malleus"
                }

            };

            return seedData;
        }
    }
}
