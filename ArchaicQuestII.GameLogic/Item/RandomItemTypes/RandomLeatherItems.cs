using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Utilities;

namespace ArchaicQuestII.GameLogic.Item.RandomItemTypes
{
    public class RandomLeatherItems
    {
        public List<PrefixItemMods> Prefix = new()
        {
            new PrefixItemMods()
            {
                Name = "Brown Leather",
                MinArmour = 3,
                MaxArmour = 7,
            },
            new PrefixItemMods()
            {
                Name = "Leather",
                MinArmour = 3,
                MaxArmour = 6
            },
            new PrefixItemMods()
            {
                Name = "Tan Leather",
                MinArmour = 3,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Embossed leather",
                MinArmour = 3,
                MaxArmour = 9
            },
            new PrefixItemMods()
            {
                Name = "Brightly coloured leather",
                MinArmour = 3,
                MaxArmour = 9
            },
            new PrefixItemMods()
            {
                Name = "Deer Leather",
                MinArmour = 3,
                MaxArmour = 3
            },
            new PrefixItemMods()
            {
                Name = "Cow Leather",
                MinArmour = 3,
                MaxArmour = 6
            },
            new PrefixItemMods()
            {
                Name = "Bull Leather",
                MinArmour = 3,
                MaxArmour = 7
            },
            new PrefixItemMods()
            {
                Name = "Moose Leather",
                MinArmour = 4,
                MaxArmour = 8
            },
            new PrefixItemMods()
            {
                Name = "Bear Leather",
                MinArmour = 5,
                MaxArmour = 10
            },
            new PrefixItemMods()
            {
                Name = "Whale leather",
                MinArmour = 5,
                MaxArmour = 11
            },
            new PrefixItemMods()
            {
                Name = "Bull elephant leather",
                MinArmour = 7,
                MaxArmour = 14
            },
            new PrefixItemMods()
            {
                Name = "dragonskin leather",
                MinArmour = 7,
                MaxArmour = 15
            },
            new PrefixItemMods()
            {
                Name = "bull moose leather",
                MinArmour = 4,
                MaxArmour = 9
            },
            new PrefixItemMods()
            {
                Name = "Shark Leather",
                MinArmour = 6,
                MaxArmour = 12
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
                Name = "Skull Cap",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Head,
                Description = new Description()
                {
                    Look = "A fitted #prefix# skull cap.",
                    Exam = "A fitted #prefix# skull cap."
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
                Name = "Trousers",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "some #prefix# trousers.",
                    Exam = "some #prefix# trousers."
                }
            },
            new Item()
            {
                Name = "Skirt",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Legs,
                Description = new Description()
                {
                    Look = "A pair of #prefix# skirt.",
                    Exam = "A pair of #prefix# skirt."
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
            },
            new Item()
            {
                Name = "Slippers",
                ArmourType = Item.ArmourTypes.Cloth,
                Slot = Equipment.EqSlot.Feet,
                Description = new Description()
                {
                    Look = "A pair #prefix# slippers.",
                    Exam = "A pair #prefix# slippers."
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
                Name = "armour",
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
            }

        };


        public Item CreateRandomItem(Player player, bool legendary)
        {
            var items = HeadItemName.Concat(LegItemName).Concat(ArmItemName).Concat(HandItemName).Concat(FeetItemName)
                .Concat(BodyItemName).ToList();
            var prefix = Prefix[DiceBag.Roll(1, 0, Prefix.Count)];
            var choice = items[DiceBag.Roll(1, 0, items.Count)];


            var item = new Item()
            {
                Name = "a " + prefix.Name + " " + choice.Name,
                ItemType = Item.ItemTypes.Armour,
                Level = player.Level,
                Value = player.Level * 75,
                Condition = DiceBag.Roll(1, 75, 100),
                Weight = 2,
                Modifier = new Modifier(),
                ArmourRating = new ArmourRating()
                {
                    Armour = DiceBag.Roll(1, prefix.MinArmour, prefix.MaxArmour),
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
               switch (DiceBag.Roll(1, 1, 16))
               {
                   case 1:
                       item.Modifier.Armour = DiceBag.Roll(1, 1, 10);
                       break;

                   case 2:
                       item.Modifier.Charisma = DiceBag.Roll(1, 1, 10);
                       break;

                   case 3:
                       item.Modifier.Constitution = DiceBag.Roll(1, 1, 10);
                       break;

                   case 4:
                       item.Modifier.Dexterity = DiceBag.Roll(1, 1, 10);
                       break;

                   case 5:
                       item.Modifier.Intelligence = DiceBag.Roll(1, 1, 10);
                       break;

                   case 6:
                       item.Modifier.Mana = DiceBag.Roll(1, 1, 10);
                       break;

                   case 7:
                       item.Modifier.Moves = DiceBag.Roll(1, 1, 10);
                       break;

                   case 8:
                       item.Modifier.Saves = DiceBag.Roll(1, 1, 10);
                       break;
                   case 9:
                       item.Modifier.Strength = DiceBag.Roll(1, 1, 10);
                       break;
                   case 10:
                       item.Modifier.Wisdom = DiceBag.Roll(1, 1, 10);
                       break;
                   case 11:
                       item.Modifier.AcMod = DiceBag.Roll(1, 1, 10);
                       break;
                   case 12:
                       item.Modifier.DamRoll = DiceBag.Roll(1, 1, 10);
                       break;
                   case 13:
                       item.Modifier.HitRoll = DiceBag.Roll(1, 1, 10);
                       break;
                   case 14:
                       item.Modifier.HP = DiceBag.Roll(1, 1, 10);
                       break;
                   case 15:
                       item.Modifier.SpellDam = DiceBag.Roll(1, 1, 10);
                       break;
                   case 16:
                       item.Modifier.AcMagicMod = DiceBag.Roll(1, 1, 10);

                       break;

               }
           }


            if (legendary)
            {
                item.ArmourRating.Armour += DiceBag.Roll(1, (int)(prefix.MinArmour * 1.5), prefix.MaxArmour * 2);
                item.ArmourRating.Magic += prefix.MaxArmour * 2 / prefix.MinArmour;
                item.Condition = 100;

                item.Name += " <span class='legendary'>(Legendary)</span>";
            }

            return item;

        }
    }
}
