using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Crafting;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Item;

public class ItemHandler : IItemHandler
{
    private readonly ConcurrentDictionary<int, CraftingRecipes> _craftingRecipesCache = new();

    private readonly RandomChainMailItems _randomChainMail = new();
    private readonly RandomClothItems _randomCloth = new();
    private readonly RandomLeatherItems _randomLeather = new();
    private readonly RandomPlateMailItems _randomPlateMail = new();
    private readonly RandomStuddedLeatherItems _randomStudded = new();
    private readonly RandomWeapons _randomWeapon = new();

    private readonly ICoreHandler _coreHandler;

    public ItemHandler(ICoreHandler coreHandler)
    {
        _coreHandler = coreHandler;
    }

    public void Init(){}

    public bool AddCraftingRecipes(int id, CraftingRecipes craftingRecipes)
    {
        return _craftingRecipesCache.TryAdd(id, craftingRecipes);
    }

    public CraftingRecipes GetCraftingRecipes(int id, CraftingRecipes recipe)
    {
        _craftingRecipesCache.TryGetValue(id, out var data);

        return data;
    }
    
    public List<CraftingRecipes> GetCraftingRecipes()
    {
        return _craftingRecipesCache.Values.ToList();
    }

    public Item CreateRandomItem(Player player, bool legendary)
    {
        var roll = DiceBag.Roll(1, 0, 5);

        return roll switch
        {
            0 => _randomWeapon.CreateRandomWeapon(player, legendary),
            1 => _randomCloth.CreateRandomItem(player, legendary),
            2 => _randomLeather.CreateRandomItem(player, legendary),
            3 => _randomStudded.CreateRandomItem(player, legendary),
            4 => _randomChainMail.CreateRandomItem(player, legendary),
            5 => _randomPlateMail.CreateRandomItem(player, legendary),
            _ => _randomWeapon.CreateRandomWeapon(player, legendary),
        };
    }
    
    public Item WeaponDrop(Player player)
    {
        var dropChance = 5;
        var roll = DiceBag.Roll(1, 1, 100);
        var legendary = false;
        
        if (roll <= dropChance)
        {
            if (roll == 1)
            {
                legendary = true;
            }

            return CreateRandomItem(player, legendary);
        }

        return null;
    }
}