using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class CraftingRecipeSeeds
    {
        internal static void SeedAndCache()
        {
            //no seeding done

            var craftingRecipes = Services.Instance.DataBase.GetList<CraftingRecipes>(
                DataBase.Collections.CraftingRecipes
            );
            foreach (var craftingRecipe in craftingRecipes)
            {
                Services.Instance.Cache.AddCraftingRecipes(craftingRecipe.Id, craftingRecipe);
            }
        }
    }
}
