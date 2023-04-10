using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class SeedData
    {
        public static void SeedAndCache()
        {
            Alignments.Seed(Services.Instance.DataBase);
            AttackTypes.Seed(Services.Instance.DataBase);
            CharacterStatuses.Seed(Services.Instance.DataBase);
            CraftingRecipeSeeds.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
            HelpFiles.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
            Items.Seed(Services.Instance.DataBase);
            Quests.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
            Races.Seed(Services.Instance.DataBase);
            Rooms.Cache(Services.Instance.DataBase, Services.Instance.Cache);
            Skills.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
            Socials.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
            InitialRoomSeed.Seed(Services.Instance.DataBase);

            ConfigOnInit.SeedAndCache(Services.Instance.DataBase, Services.Instance.Cache);
        }
    }
}
