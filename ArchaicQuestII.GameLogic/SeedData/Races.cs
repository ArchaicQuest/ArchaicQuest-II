using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Race;
using ArchaicQuestII.GameLogic.Effect;
using System;
using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Races
    {
        private static readonly List<Race> seedData = new List<Race>()
        {
            new Race()
            {
                Name = "Human",
                CreatedBy = "Malleus",
                Description = @"`Humans are highly adaptable and the most common race in the world.
They come in a wide range of skin, eye and hair colours as well as different shapes and sizes.",
                Playable = true,
                DateCreated = DateTime.Now,
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 60},
                        {EffectLocation.Dexterity, 60},
                        {EffectLocation.Constitution, 60},
                        {EffectLocation.Wisdom, 60},
                        {EffectLocation.Intelligence, 60},
                        {EffectLocation.Charisma, 60}
                    }
                }
            },
            new Race()
            {
                Name = "Elf",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
            They have an innate ability of Sneaking, Infrasion and resistance to charm spells.",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 35},
                        {EffectLocation.Dexterity, 69},
                        {EffectLocation.Constitution, 40},
                        {EffectLocation.Wisdom, 82},
                        {EffectLocation.Intelligence, 82},
                        {EffectLocation.Charisma, 75}
                    }
                }
            },
            new Race()
            {
                Name = "Wood-elf",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"wood elf",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 49},
                        {EffectLocation.Dexterity, 78},
                        {EffectLocation.Constitution, 52},
                        {EffectLocation.Wisdom, 56},
                        {EffectLocation.Intelligence, 64},
                        {EffectLocation.Charisma, 65}
                    }
                }
            },
            new Race()
            {
                Name = "Half Elf",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
            They have an innate ability of Sneaking, Infrasion and resistance to charm spells.",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 50},
                        {EffectLocation.Dexterity, 63},
                        {EffectLocation.Constitution, 49},
                        {EffectLocation.Wisdom, 69},
                        {EffectLocation.Intelligence, 69},
                        {EffectLocation.Charisma, 65}
                    }
                }
            },
            new Race()
            {
                Name = "Dark Elf",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
            They too have an innate ability of Sneaking, Infrasion and resistance to charm spells.",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 35},
                        {EffectLocation.Dexterity, 69},
                        {EffectLocation.Constitution, 42},
                        {EffectLocation.Wisdom, 81},
                        {EffectLocation.Intelligence, 81},
                        {EffectLocation.Charisma, 45}
                    }
                }
            },
            new Race()
            {
                Name = "Half Drow",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
            They too have an innate ability of Sneaking, Infrasion and resistance to charm spells.",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 45},
                        {EffectLocation.Dexterity, 63},
                        {EffectLocation.Constitution, 52},
                        {EffectLocation.Wisdom, 69},
                        {EffectLocation.Intelligence, 69},
                        {EffectLocation.Charisma, 45}
                    }
                }
            },
            new Race()
            {
                Name = "Dwarves",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
            digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
            tavern with a mug of Ale. They are powerful Warriors and Clerics",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 66},
                        {EffectLocation.Dexterity, 45},
                        {EffectLocation.Constitution, 67},
                        {EffectLocation.Wisdom, 76},
                        {EffectLocation.Intelligence, 53},
                        {EffectLocation.Charisma, 55}
                    }
                }
            },
            new Race()
            {
                Name = "Gnome",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
            digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
            tavern with a mug of Ale. They are powerful Warriors and Clerics",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 36},
                        {EffectLocation.Dexterity, 55},
                        {EffectLocation.Constitution, 46},
                        {EffectLocation.Wisdom, 71},
                        {EffectLocation.Intelligence, 92},
                        {EffectLocation.Charisma, 50}
                    }
                    }
            },
            new Race()
            {
                Name = "Ariel",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
            digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
            tavern with a mug of Ale. They are powerful Warriors and Clerics",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 60},
                        {EffectLocation.Dexterity, 70},
                        {EffectLocation.Constitution, 40},
                        {EffectLocation.Wisdom, 70},
                        {EffectLocation.Intelligence, 70},
                        {EffectLocation.Charisma, 65}
                    }
                }
            },
            new Race()
            {
                Name = "Mau",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Mau, Cat like humanoid race. Info coming soon",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 35},
                        {EffectLocation.Dexterity, 92},
                        {EffectLocation.Constitution, 45},
                        {EffectLocation.Wisdom, 55},
                        {EffectLocation.Intelligence, 70},
                        {EffectLocation.Charisma, 65}
                    }
                }
            },
            new Race()
            {
                Name = "Tlaloc",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 72},
                        {EffectLocation.Dexterity, 64},
                        {EffectLocation.Constitution, 72},
                        {EffectLocation.Wisdom, 46},
                        {EffectLocation.Intelligence, 46},
                        {EffectLocation.Charisma, 45}
                    }
                }

            },
            new Race()
            {
                Name = "Minotaur",
                CreatedBy = "Malleus",
                Playable = true,
                Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 77},
                        {EffectLocation.Dexterity, 40},
                        {EffectLocation.Constitution, 80},
                        {EffectLocation.Wisdom, 52},
                        {EffectLocation.Intelligence, 59},
                        {EffectLocation.Charisma, 40}
                    }
                }
            },
            new Race()
            {
                Name = "Other",
                CreatedBy = "Malleus",
                Description = @"`Generic race for mob is none suitable is found. you could always create one",
                Playable = false,
                Attributes = new Attributes()
                {
                    Attribute = new Dictionary<EffectLocation, int>()
                    {
                        {EffectLocation.Strength, 50},
                        {EffectLocation.Dexterity, 50},
                        {EffectLocation.Constitution, 50},
                        {EffectLocation.Wisdom, 50},
                        {EffectLocation.Intelligence, 50},
                        {EffectLocation.Charisma, 50}
                    }
                }
            },
        };

        internal static void Seed(IDataBase db)
        {
            if (!db.DoesCollectionExist(DataBase.Collections.Race))
            {
                foreach (var raceSeed in seedData)
                {
                    db.Save(raceSeed, DataBase.Collections.Race);
                }
            }
        }
    }
}
