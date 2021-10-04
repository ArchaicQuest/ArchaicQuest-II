using System;
using System.Collections.Generic;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomClothItems
    {

        private IDice _dice;

        public RandomClothItems(IDice dice)
        {
            _dice = dice;
        }
        public List<PrefixItemMods> Prefix = new List<PrefixItemMods>()
        {
            new PrefixItemMods()
            {
                Name = "Silk Cloth",
                MinArmour = 1,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Golden",
                MinArmour = 10,
                MaxArmour = 18
            },
            new PrefixItemMods()
            {
                Name = "Iridescent silk",
                MinArmour = 3,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Simple cloth",
                MinArmour = 1,
                MaxArmour = 3
            },
            new PrefixItemMods()
            {
                Name = "Embroidered wool",
                MinArmour = 4,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Strange",
                MinArmour = 6,
                MaxArmour = 12
            },
            new PrefixItemMods()
            {
                Name = "wizard",
                MinArmour = 6,
                MaxArmour = 12
            },
            new PrefixItemMods()
            {
                Name = "Green felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Red felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Blue felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Pink felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Orange felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Yellow felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Black felt",
                MinArmour = 3,
                MaxArmour = 15
            },
            new PrefixItemMods()
            {
                Name = "White felt",
                MinArmour = 3,
                MaxArmour = 15
            },
            new PrefixItemMods()
            {
                Name = "Brocade cloth",
                MinArmour = 5,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Dragonskin cloth",
                MinArmour = 10,
                MaxArmour = 20
            },
            new PrefixItemMods()
            {
                Name = "Gossamer cloth",
                MinArmour = 3,
                MaxArmour = 15
            },
        };

        public List<Item> ItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Helmet",
               ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Hood",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Hat",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Cap",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Circlet",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Crown",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Cowl",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Turban",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Helm",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Conical Hat",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Tricorne Hat",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Bicorne Hat",
                ArmourType = Item.ArmourTypes.Cloth,
            },
            new Item()
            {
                Name = "Veil",
                ArmourType = Item.ArmourTypes.Cloth,
            }
        };

        public Item CreateRandomItem(Player player, bool legendary)
        {
            var prefix = Prefix[_dice.Roll(1, 0, Prefix.Count)];
            var choice = ItemName[_dice.Roll(1, 0, ItemName.Count)];


            var item = new Item()
            {
                Name = "a " + prefix.Name + " " + choice.Name,
                ItemType = Item.ItemTypes.Armour,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = _dice.Roll(1, 75, 100),
                Weight = 2,
                ArmourRating = new ArmourRating()
                {
                     Armour = _dice.Roll(1, prefix.MinArmour, prefix.MaxArmour),
                     Magic = prefix.MaxArmour / prefix.MinArmour
                },
                Gold = player.Level * 75,
                Description = new Description()
                {
                    Look = $"a {prefix.Name} {choice.Name}",
                    Room = $"a {prefix.Name} {choice.Name} has been left here.",
                    Exam = $"a {prefix.Name} {choice.Name}",
                },
                Slot = Equipment.EqSlot.Head,
            };

            if (legendary)
            {
                item.ArmourRating.Armour += _dice.Roll(1, (int)(prefix.MinArmour * 1.5), prefix.MaxArmour * 2);
                item.ArmourRating.Magic += prefix.MaxArmour * 2 / prefix.MinArmour;
                item.Condition = 100;

                item.Name += " <span class='legendary'>(Legendary)</span>";
            }

            return item;

        }
    }
}
