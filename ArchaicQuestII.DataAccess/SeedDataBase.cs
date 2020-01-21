using System;
using System.Collections.Generic;
using System.Text;
 


namespace ArchaicQuestII.DataAccess
{
    class SeedDataBase
    {


        public SeedDataBase()
        {
            
        }


        /// <summary>
        /// Only called on application start up
        /// This is to populate the system with sensible defaults
        /// </summary>
        public void Seed()
        {
            //foreach (var data in SeedData())
            //{

            //}
        }
        //public List<Alignment> SeedData()
        //{
        //    List<Alignment> seedData = new List<Alignment>()
        //    {
        //        new Alignment()
        //        {
        //            Name = "Pure and Holy",
        //            Value = 1000,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Extremely Good",
        //            Value = 900,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Very Good",
        //            Value = 350,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Good",
        //            Value = 100,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Neutral leaning towards good",
        //            Value = -100,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Neutral",
        //            Value = -350,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Neutral leaning towards evil",
        //            Value = -600,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Evil",
        //            Value = -900,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Very evil",
        //            Value = -1000,
        //            CreatedBy = "Malleus"
        //        },
        //        new Alignment()
        //        {
        //            Name = "Pure evil",
        //            Value = -1000,
        //            CreatedBy = "Malleus"
        //        },
        //    };

        //    return seedData;
        //}


        //public List<Option> SeedData()
        //{
        //    var seedData = new List<Option>()
        //    {
        //        new Option()
        //        {
        //            Name = "Punch",
        //            CreatedBy = "Malleus"
        //        },
        //        new Option()
        //        {
        //            Name = "Pound",
        //            CreatedBy = "Malleus"
        //        },
        //        new Option()
        //        {
        //            Name = "Bite",
        //            CreatedBy = "Malleus"
        //        },
        //        new Option()
        //        {
        //            Name = "Charge",
        //            CreatedBy = "Malleus"
        //        },
        //        new Option()
        //        {
        //            Name = "Peck",
        //            CreatedBy = "Malleus"
        //        },
        //        new Option()
        //        {
        //            Name = "Headbutt",
        //            CreatedBy = "Malleus"
        //        },
        //    };

        //    return seedData;
        //}


        //private List<Model.Class> SeedData()
        //{
        //    var seedData = new List<Model.Class>()
        //    {
        //        new Model.Class()
        //        {
        //            Name = "Fighter",
        //            Description =
        //                @"Warriors are able to use any weapon and armour effectively along side their wide range of lethal and defensive combat skills.
        //            They have no need for mana, relying on their sheer strength and endurance alone to overcome opponents.
        //            Important attributes for Warriors are Strength, Dexterity and Constitution Every race can train to be an effective warrior.
        //            For beginners we recommend you pick a Human Warrior."
        //        },

        //        new Model.Class()
        //        {
        //            Name = "Thief",
        //            Description =
        //                @"Rogues are masters at the arts of remaining hidden and delivering devastating blows from the shadows before fleeing
        //             into the darkness once more. They are strong in combat but can't handle the same amount of damage as a warrior.
        //             They are also skilled lock and pocket pickers, can set or disarm traps and know how to apply poison to their blade.
        //              Rogues are a versatile class. Important attributes for Mages are Dexterity, Constitution and Strength Every race can train
        //              to be an rogue but Mau are one of the best due to their agile nature."
        //        },
        //        new Model.Class()
        //        {
        //            Name = "Cleric",
        //            Description =
        //                @"Cleric power comes from the gods they worship, stronger the devotion, stronger the power,
        //    Clerical spells focus on healing and preserving life rather than destroy it but don't be fooled clerics
        //     know powerful offensive spells to rival any mage. They can also wear any armour just like a warrior.
        //     Important attributes for Clerics are Wisdom, Intelligence and Constitution
        //      Every race can train to be a cleric but Dwarfs are one of the best."
        //        },
        //        new Model.Class()
        //        {
        //            Name = "Mage",
        //            Description =
        //                @"Mages are the most feared across the realm due to their devastating spells and power.
        //     The road to such power is a hard, slow journey. Mages struggle more than other classes in melee combat
        //      because They spent years studying magic and how to hurl a ball of fire towards their opponent instead
        //       of training for physical combat. This makes mages relatively weak at the beginning of their training
        //        but this changes however when a they have mastered the arts of magic. Important attributes for Mages
        //         are Intelligence, Wisdom and Dexterity Every race can train to be a mage but Elves are the best."
        //        }
        //    };

        //    return seedData;
        //}



//        private List<Race.Model.Race> SeedData()
//        {
//            var seedData = new List<Race.Model.Race>()
//            {
//                new Race.Model.Race()
//                {
//                    Name = "Human",
//                    CreatedBy = "Malleus",
//                    Description = @"`Humans are highly adaptable and the most common race in the world.
//They come in a wide range of skin, eye and hair colours as well as different shapes and sizes."
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Elf",
//                    CreatedBy = "Malleus",
//                    Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
//             They have an innate ability of Sneaking, Infrasion and resistance to charm spells."
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Wood-elf",
//                    CreatedBy = "Malleus",
//                    Description = @"wood elf",

//                },
//                new Race.Model.Race()
//                {
//                    Name = "Half Elf",
//                    CreatedBy = "Malleus",
//                    Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
//             They have an innate ability of Sneaking, Infrasion and resistance to charm spells."
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Dark Elf",
//                    CreatedBy = "Malleus",
//                    Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
//             They too have an innate ability of Sneaking, Infrasion and resistance to charm spells."
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Half Drow",
//                    CreatedBy = "Malleus",
//                    Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
//             They too have an innate ability of Sneaking, Infrasion and resistance to charm spells."
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Dwarves",
//                    CreatedBy = "Malleus",
//                    Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
//             digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
//              tavern with a mug of Ale. They are powerful Warriors and Clerics"
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Gnome",
//                    CreatedBy = "Malleus",
//                    Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
//             digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
//              tavern with a mug of Ale. They are powerful Warriors and Clerics"
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Ariel",
//                    CreatedBy = "Malleus",
//                    Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
//             digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
//              tavern with a mug of Ale. They are powerful Warriors and Clerics"
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Mau",
//                    CreatedBy = "Malleus",
//                    Description = @"`Mau, Cat like humanoid race. Info coming soon"
//                },
//                new Race.Model.Race()
//                {
//                    Name = "Tlaloc",
//                    CreatedBy = "Malleus",
//                    Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",

//                },
//                new Race.Model.Race()
//                {
//                    Name = "Minotaur",
//                    CreatedBy = "Malleus",
//                    Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon",

//                },
//                new Race.Model.Race()
//                {
//                    Name = "Other",
//                    CreatedBy = "Malleus",
//                    Description = @"`Generic race for mob is none suitable is found. you could always create one",
//                    Playable = false

//                },
//            };

//            return seedData;
//        }
//    }
}
}
