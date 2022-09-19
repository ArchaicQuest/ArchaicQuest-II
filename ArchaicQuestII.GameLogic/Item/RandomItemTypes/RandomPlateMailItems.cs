using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomPlateMailItems : IRandomPlateMailArmour
    {

        private IDice _dice;

        public RandomPlateMailItems(IDice dice)
        {
            _dice = dice;
        }
        public List<PrefixItemMods> Prefix = new List<PrefixItemMods>()
        {
            new PrefixItemMods()
            {
                Name = "Bronze platemail",
                MinArmour = 3,
                MaxArmour = 3,
            },
            new PrefixItemMods()
            {
                Name = "Iron platemail",
                MinArmour = 3,
                MaxArmour = 6
            },
            new PrefixItemMods()
            {
                Name = "Steel splatemail",
                MinArmour = 3,
                MaxArmour = 7
            },
            new PrefixItemMods()
            {
                Name = "Alloy platemail",
                MinArmour = 4,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Fine alloy platemail",
                MinArmour = 4,
                MaxArmour = 9
            },
            new PrefixItemMods()
            {
                Name = "Mitheril platemail",
                MinArmour = 5,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Adamantium platemail",
                MinArmour = 5,
                MaxArmour = 11
            },
            new PrefixItemMods()
            {
                Name = "Asterite platemail",
                MinArmour = 6,
                MaxArmour = 12
            },
            new PrefixItemMods()
            {
                Name = "Netherium platemail",
                MinArmour = 6,
                MaxArmour = 13
            },
            new PrefixItemMods()
            {
                Name = "Arcanium platemail",
                MinArmour = 7,
                MaxArmour = 14
            },
            new PrefixItemMods()
            {
                Name = "Dragonskin platemail",
                MinArmour = 7,
                MaxArmour = 15
            },
            new PrefixItemMods()
            {
                Name = "platemail",
                MinArmour = 3,
                MaxArmour = 5
            }
        };

        public List<Item> HeadItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Helmet",
               ArmourType = Item.ArmourTypes.Cloth,
               Slot = Equipment.EqSlot.Head,
               Description = new Description()
               {
                   Look = "A fitted #prefix# helmet.",
                   Exam = "A fitted #prefix# helmet."
               }
            },
            new Item()
            {
                Name = "Coif",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A simple #prefix# coif that protects the neck and shoulders.",
                    Exam = "A simple #prefix# coif that protects the neck and shoulders."
                }
            },
            new Item()
            {
                Name = "Helm",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A fitted #prefix# helm.",
                    Exam = "A fitted #prefix# helm."
                }
            }
        };
        public List<Item> LegItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Leggings",
               ArmourType = Item.ArmourTypes.Cloth,
               Slot = Equipment.EqSlot.Legs,
               Description = new Description()
               {
                   Look = "A pair of #prefix# leggings",
                   Exam = "A pair of #prefix# leggings",
               }
            },
            new Item()
            {
                Name = "Greaves",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "Some #prefix# greaves.",
                    Exam = "Some #prefix# greaves.",
                }
            },
            new Item()
            {
                Name = "Skirt",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "A protective #prefix# skirt.",
                    Exam = "A protective #prefix# skirt."
                }
            }

        };
        public List<Item> ArmItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Sleeves",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Arms,
                Description = new Description()
                {
                    Look = "A pair of #prefix# sleeves",
                    Exam = "A pair of #prefix# sleeves",
                }
            },
            new Item()
            {
                Name = "Armbands",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Arms,
                Description = new Description()
                {
                    Look = "A pair #prefix# armbands.",
                    Exam = "A pair #prefix# armbands."
                }
            },
            new Item()
            {
                Name = "Armplates",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Arms,
                Description = new Description()
                {
                    Look = "A pair #prefix# Armplates.",
                    Exam = "A pair #prefix# Armplates."
                }
            }

        };
        public List<Item> HandItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Gloves",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Hands,
                Description = new Description()
                {
                    Look = "A pair of #prefix# gloves",
                    Exam = "A pair of #prefix# gloves",
                }
            },
            new Item()
            {
                Name = "Gauntlets",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Hands,
                Description = new Description()
                {
                    Look = "A pair of #prefix# gauntlets",
                    Exam = "A pair of #prefix# gauntlets",
                }
            },
        };
        public List<Item> FeetItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Boots",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Feet,
                Description = new Description()
                {
                    Look = "A pair of #prefix# boots",
                    Exam = "A pair of #prefix# boots",
                }
            },


        };
        public List<Item> BodyItemName = new List<Item>()
        {


            new Item()
            {
                Name = "Jerkin",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# jerkin.",
                    Exam = "A #prefix# jerkin."
                }
            },
            new Item()
            {
                Name = "Armour",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# armour.",
                    Exam = "A #prefix# armour."
                }
            },
            new Item()
            {
                Name = "Tunic",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# tunic.",
                    Exam = "A #prefix# tunic."
                }
            },
            new Item()
            {
                Name = "Vest",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# vest.",
                    Exam = "A #prefix# vest."
                }
            },
            new Item()
            {
                Name = "Jacket",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# jacket.",
                    Exam = "A #prefix# jacket."
                }
            },

        };


        public Item CreateRandomItem(Player player, bool legendary)
        {
            var items = HeadItemName.Concat(LegItemName).Concat(ArmItemName).Concat(HandItemName).Concat(FeetItemName)
                .Concat(BodyItemName).ToList();
            var prefix = Prefix[_dice.Roll(1, 0, Prefix.Count)];
            var choice = items[_dice.Roll(1, 0, items.Count)];


            var item = new Item()
            {
                Name = "a " + prefix.Name + " " + choice.Name,
                ItemType = Item.ItemTypes.Armour,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = _dice.Roll(1, 75, 100),
                Weight = 2,
                Modifier = new Modifier(),
                ArmourRating = new ArmourRating()
                {
                    Armour = _dice.Roll(1, prefix.MinArmour, prefix.MaxArmour),
                    Magic = prefix.MaxArmour / prefix.MinArmour
                },
                Gold = player.Level * 75,
                Description = new Description()
                {
                    Look = $"{choice.Description.Look.Replace("#prefix#", prefix.Name)}",
                    Room = $"a {prefix.Name} {choice.Name} has been left here.",
                    Exam = $"{choice.Description.Exam}",
                },
                Slot = choice.Slot,
            };
        
           // stats to buff

           for (int i = 0; i < (legendary ? 5 : 3); i++)
           {
                  switch (_dice.Roll(1, 1, 16))
           {
               case 1:
                   item.Modifier.Armour = _dice.Roll(1, 1, 10);
                   break;
               
               case 2:
                   item.Modifier.Charisma = _dice.Roll(1, 1, 10);
                   break;
               
               case 3:
                   item.Modifier.Constitution = _dice.Roll(1, 1, 10);
                   break;
               
               case 4:
                   item.Modifier.Dexterity = _dice.Roll(1, 1, 10);
                   break;
               
               case 5:
                   item.Modifier.Intelligence = _dice.Roll(1, 1, 10);
                   break;
               
               case 6:
                   item.Modifier.Mana = _dice.Roll(1, 1, 10);
                   break;
               
               case 7:
                   item.Modifier.Moves = _dice.Roll(1, 1, 10);
                   break;
               
               case 8:
                   item.Modifier.Saves = _dice.Roll(1, 1, 10);
                   break;
               case 9:
                   item.Modifier.Strength = _dice.Roll(1, 1, 10);
                   break;
               case 10:
                   item.Modifier.Wisdom = _dice.Roll(1, 1, 10);
                   break;
               case 11:
                   item.Modifier.AcMod = _dice.Roll(1, 1, 10);
                   break;
               case 12:
                   item.Modifier.DamRoll = _dice.Roll(1, 1, 10);
                   break;
               case 13:
                   item.Modifier.HitRoll = _dice.Roll(1, 1, 10);
                   break;
               case 14:
                   item.Modifier.HP = _dice.Roll(1, 1, 10);
                   break;
               case 15:
                   item.Modifier.SpellDam = _dice.Roll(1, 1, 10);
                   break;
               case 16:
                   item.Modifier.AcMagicMod = _dice.Roll(1, 1, 10);
                   
                   break;
                
           }
           }


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
