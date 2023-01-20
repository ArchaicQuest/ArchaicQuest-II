using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Crafting;

namespace ArchaicQuestII.GameLogic.Item;

public interface IItemHandler
{
    void Init();
    bool AddCraftingRecipes(int id, CraftingRecipes CraftingRecipes);
    CraftingRecipes GetCraftingRecipes(int id, CraftingRecipes recipe);
    List<CraftingRecipes> GetCraftingRecipes();
    Item WeaponDrop(Player player);
    Item CreateRandomItem(Player player, bool legendary);

}