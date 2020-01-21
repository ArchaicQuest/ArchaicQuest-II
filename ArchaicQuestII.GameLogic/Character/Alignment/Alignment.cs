using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Character.Alignment
{
    public class Alignment: Option, IAlignment
    {
        /// <summary>
        /// Value determines Alignment value
        ///   
        ///Evil    - Alignment between and including -1000 and -350
        ///Neutral - Alignment between and including -349 and 349
        ///Good    - Alignment between and including 350 and 1000
        /// </summary>
        public int Value { get; set; }

        private IDataBase _db { get; }

        public Alignment()
        {
            
        }

        public Alignment(IDataBase db)
        {
            _db = db;
        }

        public void CreateAlignment(IAlignment option)
        {
            _db.Save(option, "Alignment");

            // TODO: check
            //col.Insert(option);
            //col.EnsureIndex(x => x.Name);
        }

        public Alignment GetAlignment(int id)
        {
            return _db.GetById<Alignment>(id, "Alignment");
        }

        public List<Alignment> GetAlignments()
        {
            return _db.GetCollection<Alignment>("Alignment").FindAll().OrderBy(x => x.Name).ToList();
        }

        /// <summary>
        /// Only called on application start up
        /// if alignment data already exists this wont run.
        /// This is to populate the system with sensible defaults
        /// </summary>
        public void Seed()
        {
            foreach (var data in SeedData())
            {
                CreateAlignment(data);
            }
        }

        public List<Alignment> SeedData()
        {
            List<Alignment> seedData = new List<Alignment>()
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
