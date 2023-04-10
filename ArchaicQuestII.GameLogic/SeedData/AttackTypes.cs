using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class AttackTypes
    {
        private static readonly List<Option> seedData = new List<Option>()
        {
            new Option() { Name = "Punch", CreatedBy = "Malleus" },
            new Option() { Name = "Pound", CreatedBy = "Malleus" },
            new Option() { Name = "Bite", CreatedBy = "Malleus" },
            new Option() { Name = "Charge", CreatedBy = "Malleus" },
            new Option() { Name = "Peck", CreatedBy = "Malleus" },
            new Option() { Name = "Headbutt", CreatedBy = "Malleus" },
        };

        internal static void Seed()
        {
            if (!Services.Instance.DataBase.DoesCollectionExist(DataBase.Collections.AttackType))
            {
                foreach (var attackTypeSeed in seedData)
                {
                    Services.Instance.DataBase.Save(
                        attackTypeSeed,
                        DataBase.Collections.AttackType
                    );
                }
            }
        }
    }
}
