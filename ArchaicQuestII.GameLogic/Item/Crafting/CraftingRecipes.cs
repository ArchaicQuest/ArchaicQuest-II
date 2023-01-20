using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Crafting
{
    public class CraftingRecipes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CraftingMaterials> CraftingMaterials { get; set; } = new();
        public Item.Item CreatedItem { get; set; }
        public bool CreatedItemDropsInRoom { get; set; }
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
