using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchaicQuestII.GameLogic.Item
{
    public class ItemObj
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class ItemList: List<Item>, IItemList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Item.ItemTypes Type { get; set; }
        public int Value { get; set; }
        public int CountOfItems { get; set; }
        public IEnumerable<ItemObj> List(bool isRoom = true)
        {
            return List(this, isRoom);
        }

        public IEnumerable<ItemObj> List(IEnumerable<Item> items, params object[] args)
        {
            return items.Where(x => x.IsHiddenInRoom == false).GroupBy(t => new
            {
                t.Id,
                t.Name,
                t.ItemType,
                t.Value,
                t.Description.Room
            }).Select(groupedItem => new ItemList()
            {
                Id = groupedItem.Key.Id,
                Type = groupedItem.Key.ItemType,
                Value = groupedItem.Key.Value,
                Name = (bool)args[0] ? groupedItem.Key.Room.Replace("{name}", groupedItem.Key.Name) 
                : groupedItem.Key.Name,
                CountOfItems = groupedItem.Count()
            }).Select(x =>
            {
                if (x.Type == Item.ItemTypes.Money)
                {
                    
                    return new ItemObj() {Name = DisplayMoneyAmount(x.Value), Id = x.Id};
                }

                var itemString = x.CountOfItems > 1 ? $"({x.CountOfItems}) {x.Name}" : x.Name;

                return new ItemObj(){ Name = itemString, Id = x.Id};
            });
        }

        public static string DisplayMoneyAmount(int countOfCoin)
        {
            if (countOfCoin == 1)
            {
                return "A single gold coin";
            }

            if (countOfCoin == 2)
            {
                return "A couple of gold coins.";
            }

            if (countOfCoin > 2 && countOfCoin < 100)
            {
                return "A few gold coins.";
            }

            if (countOfCoin >= 100 && countOfCoin < 300)
            {
                return "A small pile of gold coins.";
            }

            if (countOfCoin >= 300 && countOfCoin < 1000)
            {
                return "A pile of gold coins.";
            }

            return "A large pile of gold coins.";
        }
    }
}
