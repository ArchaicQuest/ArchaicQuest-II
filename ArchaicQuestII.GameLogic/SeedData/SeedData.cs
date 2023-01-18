using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class SeedData
    {
        public static void SeedAndCache(
            ICoreHandler coreHandler)
        {
            Alignments.Seed(coreHandler.Db);
            AttackTypes.Seed(coreHandler.Db);
            CharacterStatuses.Seed(coreHandler.Db);
            Classes.SeedAndCache(coreHandler.Db, coreHandler.Character);
            CraftingRecipeSeeds.SeedAndCache(coreHandler.Db, coreHandler.Item);
            HelpFiles.SeedAndCache(coreHandler.Db, coreHandler.Command);
            Items.Seed(coreHandler.Db);
            Quests.SeedAndCache(coreHandler.Db, coreHandler.Character);
            Races.Seed(coreHandler.Db);
            Rooms.Cache(coreHandler.Db, coreHandler.World);
            Skills.SeedAndCache(coreHandler.Db, coreHandler.Command);
            Socials.SeedAndCache(coreHandler.Db, coreHandler.Command);
            InitialRoomSeed.Seed(coreHandler.Db);

            ConfigOnInit.SeedAndCache(coreHandler);
        }
    }
}