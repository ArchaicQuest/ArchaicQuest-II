using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class CraftingRecipeSeeds
    {
        internal static void SeedAndCache(IDataBase db, IItemHandler itemHandler)
        {
            //no seeding done

            var craftingRecipes = db.GetList<CraftingRecipes>(DataBase.Collections.CraftingRecipes);
            foreach (var craftingRecipe in craftingRecipes)
            {
                itemHandler.AddCraftingRecipes(craftingRecipe.Id, craftingRecipe);
            }
        }
    }
}
