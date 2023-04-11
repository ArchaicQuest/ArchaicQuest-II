namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class SeedData
    {
        public static void SeedAndCache()
        {
            Alignments.Seed();
            AttackTypes.Seed();
            CharacterStatuses.Seed();
            CraftingRecipeSeeds.SeedAndCache();
            HelpFiles.SeedAndCache();
            Items.Seed();
            Quests.SeedAndCache();
            Races.Seed();
            Rooms.Cache();
            Skills.SeedAndCache();
            Socials.SeedAndCache();
            InitialRoomSeed.Seed();

            ConfigOnInit.SeedAndCache();
        }
    }
}
