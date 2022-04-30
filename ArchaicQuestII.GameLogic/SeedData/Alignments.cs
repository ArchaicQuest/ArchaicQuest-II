using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Alignment;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Alignments
    {
        private static readonly List<Alignment> seedData = new List<Alignment>()
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

        internal static void Seed(IDataBase db) 
        {
            if (!db.DoesCollectionExist(DataBase.Collections.Alignment))
            {
                foreach (var alignmentSeed in seedData)
                {
                    db.Save(alignmentSeed, DataBase.Collections.Alignment);
                }
            }
        }
    }
}