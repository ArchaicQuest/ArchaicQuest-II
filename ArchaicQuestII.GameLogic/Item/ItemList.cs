using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    

    public class ItemList: List<Item>, IItemList
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public IEnumerable<string> List()
        {
            return List(this);
        }

        public IEnumerable<string> List(IEnumerable<Item> items)
        {
            return items.GroupBy(t => new
            {
                t.Name,
                t.Description.Room
            }).Select(groupedItem => new ItemList()
            {
                Name = groupedItem.Key.Room.Replace("{name}", groupedItem.Key.Name),
                Count = groupedItem.Count()
            }).Select(x =>
            {
                var itemString = x.Count > 1 ? $"({x.Count}) {x.Name}" : x.Name;

                return itemString;
            });
        }
    }
}
