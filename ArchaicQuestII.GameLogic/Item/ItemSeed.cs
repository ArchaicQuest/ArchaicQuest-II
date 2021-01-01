using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character.Equipment;

namespace ArchaicQuestII.GameLogic.Item
{
    public class ItemSeed
    {
        public List<Item> SeedData()
        {
            var seedData = new List<Item>()
            {
                new Item()
                {
                    Name = "Gold Coin",
                    Value = 1,
                    ItemType = Item.ItemTypes.Money,
                    ArmourType = Item.ArmourTypes.Cloth,
                    AttackType = Item.AttackTypes.Charge,
                    WeaponType = Item.WeaponTypes.Arrows,
                    Gold = 1,
                    Slot = Equipment.EqSlot.Hands,
                    Level = 1,
                    Modifier = new Modifier(),
                    Description = new Description()
                    {
                        Look = "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                        Exam =  "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                        Room =  "A single gold coin.",
                    },
                    Book = new Book()
                    {
                        Pages = new List<string>()
                    },
                    ArmourRating = new ArmourRating(),
                    Container = new Container()
                    {
                        Items = new ItemList()
                    }
                    
                }
            };

            return seedData;
        }
    }
}
