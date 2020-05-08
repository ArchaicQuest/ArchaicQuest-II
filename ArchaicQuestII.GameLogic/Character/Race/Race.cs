
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using LiteDB;

namespace ArchaicQuestII.GameLogic.Character.Race
{
    public class Race : OptionDescriptive
    {
        [BsonField("p")]
        public bool Playable { get; set; }
       // [BsonField("a")]
      //  public Attributes Attributes { get; set; } = new Attributes();

        public List<Race> SeedData()
        {
            var seedData = new List<Race>()
                    {
                        new Race()
                        {
                            Name = "Human",
                            CreatedBy = "Malleus",
                            Description = @"`Humans are highly adaptable and the most common race in the world.
        They come in a wide range of skin, eye and hair colours as well as different shapes and sizes.",
                        },
                        new Race()
                        {
                            Name = "Elf",
                            CreatedBy = "Malleus",
                            Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
                     They have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                        },
                        new Race()
                        {
                            Name = "Wood-elf",
                            CreatedBy = "Malleus",
                            Description = @"wood elf",

                        },
                        new Race()
                        {
                            Name = "Half Elf",
                            CreatedBy = "Malleus",
                            Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
                     They have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                        },
                        new Race()
                        {
                            Name = "Dark Elf",
                            CreatedBy = "Malleus",
                            Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
                     They too have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                        },
                        new Race()
                        {
                            Name = "Half Drow",
                            CreatedBy = "Malleus",
                            Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
                     They too have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                        },
                        new Race()
                        {
                            Name = "Dwarves",
                            CreatedBy = "Malleus",
                            Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
                     digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
                      tavern with a mug of Ale. They are powerful Warriors and Clerics"
                        },
                        new Race()
                        {
                            Name = "Gnome",
                            CreatedBy = "Malleus",
                            Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
                     digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
                      tavern with a mug of Ale. They are powerful Warriors and Clerics"
                        },
                        new Race()
                        {
                            Name = "Ariel",
                            CreatedBy = "Malleus",
                            Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
                     digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
                      tavern with a mug of Ale. They are powerful Warriors and Clerics"
                        },
                        new Race()
                        {
                            Name = "Mau",
                            CreatedBy = "Malleus",
                            Description = @"`Mau, Cat like humanoid race. Info coming soon"
                        },
                        new Race()
                        {
                            Name = "Tlaloc",
                            CreatedBy = "Malleus",
                            Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",

                        },
                        new Race()
                        {
                            Name = "Minotaur",
                            CreatedBy = "Malleus",
                            Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",

                        },
                        new Race()
                        {
                            Name = "Other",
                            CreatedBy = "Malleus",
                            Description = @"`Generic race for mob is none suitable is found. you could always create one",
                            Playable = false

                        },
                    };

            return seedData;
        }
    }


}
