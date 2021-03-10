using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Crafting
{
    public interface ICooking
    {
        public void Cook(Player player, Room room);
        public Item.Item GenerateCookedItem(Player player, Room room, List<Tuple<Item.Item, int>> ingredients);
        public int CalculateModifer(IOrderedEnumerable<Tuple<Item.Item, int>> ingredients, string name);

    }
}
