using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Item;
using System.Collections.Generic;
using System.Linq;

namespace ArchaicQuestII.GameLogic.SeedData
{
    public static class Items
    {
        public static readonly List<Item.Item> seedData = new List<Item.Item>()
        {
            new Item.Item()
            {
                Name = "Gold Coin",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Money,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Charge,
                WeaponType = Character.SkillName.Throw,
                Gold = 1,
                Slot = Equipment.EqSlot.Hands,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                    Exam =
                        "A small gold coin with an embossed crown on one side and the number one on the opposite side, along the edge inscribed is 'de omnibus dubitandum'",
                    Room = "A single gold coin.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating(),
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "The torch of illuminatio",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.Throw,
                Gold = 1,
                Slot = Equipment.EqSlot.Light,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "The Illuminatio is a magical torch carved from a smooth light wood. The flame that burns gives off no heat and glows yellow in colour. The words Dominus Illuminatio Mea are carved down the side of the torch",
                    Exam =
                        "The Illuminatio is a magical torch carved from a smooth light wood. The flame that burns gives off no heat and glows yellow in colour. The words Dominus Illuminatio Mea are carved down the side of the torch",
                    Room = "A magical torch.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating(),
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A simple cloth robe",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.None,
                Gold = 1,
                Slot = Equipment.EqSlot.Torso,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "This is a very simple cloth robe, undyed and scruffy. It provides some warmth and very little defense.",
                    Exam =
                        "This is a very simple cloth robe, undyed and scruffy. It provides some warmth and very little defense.",
                    Room = "A ragged shirt has been discarded here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating() { Armour = 1 },
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A ragged shirt",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.None,
                Gold = 1,
                Slot = Equipment.EqSlot.Torso,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "This is a very plain shirt, undyed and scruffy with a few buttons missing. It provides some warmth and very little defense.",
                    Exam =
                        "This is a very plain shirt, undyed and scruffy with a few buttons missing. It provides some warmth and very little defense.",
                    Room = "A ragged shirt has been discarded here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating() { Armour = 1 },
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A pair of baggy sleeves",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.None,
                Gold = 1,
                Slot = Equipment.EqSlot.Arms,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "These are scruffy looking baggy sleeves that provide some warmth and very little defense.",
                    Exam =
                        "These are scruffy looking baggy sleeves that provide some warmth and very little defense.",
                    Room = "A pair of baggy sleeves has been discarded here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating() { Armour = 1 },
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A pair of baggy trousers",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.None,
                Gold = 1,
                Slot = Equipment.EqSlot.Legs,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "These baggy trousers are a dull brown with a few stains and the knees worn in, the bottom trouser legs are frayed in several places, it just about provides protection against the elements.",
                    Exam =
                        "These baggy trousers are a dull brown with a few stains and the knees worn in, the bottom trouser legs are frayed in several places, it just about provides protection against the elements.",
                    Room = "A pair of baggy trousers have been discarded here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating() { Armour = 1 },
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A pair of worn leather boots",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Armour,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Crush,
                WeaponType = Character.SkillName.None,
                Gold = 1,
                Slot = Equipment.EqSlot.Feet,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look =
                        "Dirty Brown boots covered in scuff marks with worn soles. These boots look old and dull in colour",
                    Exam =
                        "Dirty Brown boots covered in scuff marks with worn soles. These boots look old and dull in colour",
                    Room = "A pair of worn leather boots have been left here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating() { Armour = 1 },
                Container = new Container() { Items = new ItemList() }
            },
            new Item.Item()
            {
                Name = "A simple iron dagger",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Weapon,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Stab,
                WeaponType = Character.SkillName.ShortBlades,
                Gold = 1,
                Slot = Equipment.EqSlot.Wielded,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look = "A basic iron dagger you see nothing special about it",
                    Exam = "A basic iron dagger you see nothing special about it",
                    Room = "A basic iron dagger left here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating(),
                Container = new Container() { Items = new ItemList() },
                Damage = new Item.Damage() { Minimum = 1, Maximum = 4 }
            },
            new Item.Item()
            {
                Name = "A simple iron mace",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Weapon,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Stab,
                WeaponType = Character.SkillName.Hammer,
                Gold = 1,
                Slot = Equipment.EqSlot.Wielded,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look = "A basic iron mace you see nothing special about it",
                    Exam = "A basic iron mace you see nothing special about it",
                    Room = "A basic iron mace left here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating(),
                Container = new Container() { Items = new ItemList() },
                Damage = new Item.Damage() { Minimum = 1, Maximum = 4 }
            },
            new Item.Item()
            {
                Name = "A simple iron sword",
                Value = 1,
                ItemType = Item.Item.ItemTypes.Weapon,
                ArmourType = Item.Item.ArmourTypes.Cloth,
                AttackType = Item.Item.AttackTypes.Stab,
                WeaponType = Character.SkillName.LongBlades,
                Gold = 1,
                Slot = Equipment.EqSlot.Wielded,
                Level = 1,
                Modifier = new Modifier(),
                Description = new Description()
                {
                    Look = "A basic iron sword you see nothing special about it",
                    Exam = "A basic iron sword you see nothing special about it",
                    Room = "A basic iron sword left here.",
                },
                Book = new Book() { Pages = new List<string>() },
                ArmourRating = new ArmourRating(),
                Container = new Container() { Items = new ItemList() },
                Damage = new Item.Damage() { Minimum = 1, Maximum = 4 }
            },
        };

        internal static void Seed()
        {
            if (!Services.Instance.DataBase.DoesCollectionExist(DataBase.Collections.Items))
            {
                foreach (var itemSeed in seedData)
                {
                    Services.Instance.DataBase.Save(itemSeed, DataBase.Collections.Items);
                }

                return;
            }

            var hasMoney = Services.Instance.DataBase
                .GetList<Item.Item>(DataBase.Collections.Items)
                .Any(x => x.ItemType == Item.Item.ItemTypes.Money);

            if (!hasMoney)
            {
                foreach (var itemSeed in seedData)
                {
                    Services.Instance.DataBase.Save(itemSeed, DataBase.Collections.Items);
                }
            }
        }
    }
}
