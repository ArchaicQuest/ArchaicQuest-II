using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class CraftingRecipeSeeds
    {
        internal static void SeedAndCache(IDataBase db, ICache cache)
        {
            //no seeding done

            var craftingRecipes = db.GetList<CraftingRecipes>(DataBase.Collections.CraftingRecipes);
            foreach (var craftingRecipe in craftingRecipes)
            {
                cache.AddCraftingRecipes(craftingRecipe.Id, craftingRecipe);
            }
        }
    }
}
