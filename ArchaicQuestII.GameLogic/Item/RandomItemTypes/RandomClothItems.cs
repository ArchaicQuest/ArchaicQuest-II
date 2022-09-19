using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomClothItems : IRandomClothItems
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
                MinArmour = 4,
                MaxArmour = 8,
            },
            new PrefixItemMods()
            {
                Name = "Golden silk",
                MinArmour = 6,
                MaxArmour = 12
            },
            new PrefixItemMods()
            {
                Name = "Sylvan Cloth",
                MinArmour = 5,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Seamist Cloth",
                MinArmour = 5,
                MaxArmour = 11
            },
            new PrefixItemMods()
            {
                Name = "Woolen Cloth",
                MinArmour = 3,
                MaxArmour = 3
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
                MinArmour = 3,
                MaxArmour = 6
            },
            new PrefixItemMods()
            {
                Name = "Strange",
                MinArmour = 3,
                MaxArmour = 7
            },
            new PrefixItemMods()
            {
                Name = "wizard",
                MinArmour = 3,
                MaxArmour = 8
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
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "White felt",
                MinArmour = 3,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Brocade cloth",
                MinArmour = 3,
                MaxArmour = 7
            },
            new PrefixItemMods()
            {
                Name = "Dragonskin cloth",
                MinArmour = 7,
                MaxArmour = 15
            },
            new PrefixItemMods()
            {
                Name = "Gossamer cloth",
                MinArmour = 4,
                MaxArmour = 9
            },
            new PrefixItemMods()
            {
                Name = "Linen cloth",
                MinArmour = 3,
                MaxArmour = 6
            },
            new PrefixItemMods()
            {
                Name = "Nightshade cloth",
                MinArmour = 6,
                MaxArmour = 12
            },
            new PrefixItemMods()
            {
                Name = "Wyvernskin cloth",
                MinArmour = 6,
                MaxArmour = 13
            },
            new PrefixItemMods()
            {
                Name = "Silksteel cloth",
                MinArmour = 7,
                MaxArmour = 14
            },
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
                Name = "Hood",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A large #prefix# hood.",
                    Exam = "A large #prefix# hood."
                }
            },
            new Item()
            {
                Name = "Hat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A simple #prefix# hat.",
                    Exam = "A simple #prefix# hat."
                }
            },
            new Item()
            {
                Name = "Cap",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A fitted #prefix# cap.",
                    Exam = "A fitted #prefix# cap."
                }
            },
            new Item()
            {
                Name = "Cowl",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A soft #prefix# cowl.",
                    Exam =  "A soft #prefix# cowl.",
                }
            },
            new Item()
            {
                Name = "Turban",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A #prefix# turban.",
                    Exam = "A #prefix# turban.",
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
            },
            new Item()
            {
                Name = "Conical Hat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A #prefix# conical Hat with a wide brim.",
                    Exam = "A #prefix# conical Hat with a wide brim."
                }
            },
            new Item()
            {
                Name = "Tricorne Hat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A #prefix# tricorne hat.",
                    Exam = "A #prefix# tricorne hat with a broad brim, pinned up on either side of the head and at the back, producing a triangular shape."
                }
            },
            new Item()
            {
                Name = "Bicorne Hat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A #prefix# bicorne hat.",
                    Exam = "A #prefix# bicorne hat with a broad brim, the front and the rear halves turned up and pinned together forming a semi-circular fan shape."
                }
            },
            new Item()
            {
                Name = "Veil",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A delicate #prefix# veil.",
                    Exam = "A delicate #prefix# veil."
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
                Name = "pantaloons",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "some #prefix# pantaloons.",
                    Exam = "some #prefix# pantaloons."
                }
            },
            new Item()
            {
                Name = "baggy trousers",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "A pair of #prefix# baggy trousers.",
                    Exam = "A pair of #prefix# baggy trousers."
                }
            },
            new Item()
            {
                Name = "tights",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "A pair of #prefix# tights.",
                    Exam = "A pair of #prefix# tights.",
                }
            },
            new Item()
            {
                Name = "trousers",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "A pair of #prefix# trousers.",
                    Exam = "A pair of  #prefix# trousers."
                }
            },

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
                Name = "armbands",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Arms,
                Description = new Description()
                {
                    Look = "A pair #prefix# armbands.",
                    Exam = "A pair  #prefix# armbands."
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
            new Item()
            {
                Name = "shoes",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Feet,
                Description = new Description()
                {
                    Look = "A pair #prefix# shoes.",
                    Exam = "A pair #prefix# shoes."
                }
            },
            new Item()
            {
                Name = "Knee-high boots",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Feet,
                Description = new Description()
                {
                    Look = "A pair #prefix# Knee-high boots.",
                    Exam = "A pair #prefix# Knee-high boots."
                }
            },
            new Item()
            {
                Name = "Moccasins",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Feet,
                Description = new Description()
                {
                    Look = "A pair #prefix# Moccasins.",
                    Exam = "A pair #prefix# Moccasins."
                }
            }

        };
        public List<Item> BodyItemName = new List<Item>()
        {

            new Item()
            {
                Name = "Shirt",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# shirt",
                    Exam = "A #prefix# shirt",
                }
            },
            new Item()
            {
                Name = "Robe",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# robe.",
                    Exam = "A #prefix# robe."
                }
            },
            new Item()
            {
                Name = "Dress",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# dress.",
                    Exam = "A #prefix# dress."
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
                Name = "Cape",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# cape.",
                    Exam = "A #prefix# cape."
                }
            },
            new Item()
            {
                Name = "Coat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# Coat.",
                    Exam = "A #prefix# Coat."
                }
            },
            new Item()
            {
                Name = "Tunic",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# Tunic.",
                    Exam = "A #prefix# Tunic."
                }
            },
            new Item()
            {
                Name = "Smock",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# Smock.",
                    Exam = "A #prefix# Smock."
                }
            },
            new Item()
            {
                Name = "Gothic tailcoat",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Body,
                Description = new Description()
                {
                    Look = "A #prefix# gothic tailcoat.",
                    Exam = "A #prefix# gothic tailcoat."
                }
            }

        };


        public Item CreateRandomItem(Player player, bool legendary)
        {

            var items = HeadItemName.Concat(LegItemName).Concat(ArmItemName).Concat(HandItemName).Concat(FeetItemName)
                .Concat(BodyItemName).ToList();
            var prefix = Prefix[_dice.Roll(1, 0, Prefix.Count)];
            var choice = items[_dice.Roll(1, 0, items.Count)];

            var name = "a " + prefix.Name + " " + choice.Name;

            if (choice.Slot == Equipment.EqSlot.Hands || choice.Slot == Equipment.EqSlot.Legs || choice.Slot == Equipment.EqSlot.Arms)
            {
                var pairOrSome = _dice.Roll(1, 1, 2);
                name = $"{(pairOrSome == 1 ? "A pair of " : "some ")}" + prefix.Name + " " + choice.Name;
            }

            if (choice.Slot == Equipment.EqSlot.Legs && choice.Name.StartsWith("skirt", StringComparison.CurrentCultureIgnoreCase))
            {
                name = "a " + prefix.Name + " " + choice.Name;
            }

            var item = new Item()
            {
                Name = name.ToLower(),
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
