using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class CharacterStatuses
    {
        private static readonly List<Option> seedData = new List<Option>()
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

        internal static void Seed(IDataBase db)
        {
            if (!db.DoesCollectionExist(DataBase.Collections.Status))
            {
                foreach (var charStatusSeed in seedData)
                {
                    db.Save(charStatusSeed, DataBase.Collections.Status);
                }
            }
        }
    }
}
