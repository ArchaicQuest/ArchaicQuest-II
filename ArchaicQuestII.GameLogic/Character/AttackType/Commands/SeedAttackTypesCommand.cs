using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class SeedAttackTypesCommand
    {
        private IDataBase _db { get; }
        public SeedAttackTypesCommand(IDataBase db)
        {
            _db = db;
        }

        public void Seed()
        {

           

                var command = new CreateAttackTypeCommand(_db);

                foreach (var data in SeedData())
                {
                    command.CreateAttackType(data);
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
