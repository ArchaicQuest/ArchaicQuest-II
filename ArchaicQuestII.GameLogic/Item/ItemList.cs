using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    

    public class ItemList: List<Item>, IItemList
    {
        public string Name { get; set; }
        public int CountOfItems { get; set; }
        public IEnumerable<string> List(bool isRoom = true)
        {
            return List(this, isRoom);
        }

        public IEnumerable<string> List(IEnumerable<Item> items, params object[] args)
        {
            return items.GroupBy(t => new
            {
                t.Name,
                t.Description.Room
            }).Select(groupedItem => new ItemList()
            {
                Name = (bool)args[0] ? groupedItem.Key.Room.Replace("{name}", groupedItem.Key.Name) 
                : groupedItem.Key.Name,
                CountOfItems = groupedItem.Count()
            }).Select(x =>
            {
                var itemString = x.CountOfItems > 1 ? $"({x.CountOfItems}) {x.Name}" : x.Name;

                return itemString;
            });
        }
    }
}
