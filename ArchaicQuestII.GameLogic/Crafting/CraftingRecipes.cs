using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Crafting
{

    public class CraftingMaterials
    {
        public string Material { get; set; }
        public int Quantity { get; set; }
        public bool PresentInRoom { get; set; }
    }

    public class CraftingRecipes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CraftingMaterials> CraftingMaterials { get; set; } = new List<CraftingMaterials>();
        public Item.Item CreatedItem { get; set; }
        public bool CreatedItemDropsInRoom { get; set; } = false;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
