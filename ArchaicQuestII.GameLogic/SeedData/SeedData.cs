using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class SeedData
    {
        public static void SeedAndCache(IDataBase db, ICache cache)
        {
            Alignments.Seed(db);
            AttackTypes.Seed(db);
            CharacterStatuses.Seed(db);
            Classes.SeedAndCache(db, cache);
            CraftingRecipeSeeds.SeedAndCache(db, cache);
            HelpFiles.SeedAndCache(db, cache);
            Items.Seed(db);
            Quests.SeedAndCache(db, cache);
            Races.Seed(db);
            Rooms.Cache(db, cache);
            Skills.SeedAndCache(db, cache);
            Socials.SeedAndCache(db, cache);
            InitialRoomSeed.Seed(db);

            ConfigOnInit.SeedAndCache(db, cache);
        }
    }
}