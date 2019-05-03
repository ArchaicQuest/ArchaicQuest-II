using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Race.Commands;
using ArchaicQuestII.Engine.Character.Class.Commands;
using ArchaicQuestII.Engine.Core.Interface;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Commands
{
    public class SeedRaceCommand
    {


        public void Seed()
        {

            using (var db = new LiteDatabase(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyData.db")))
            {
                if (db.CollectionExists("Race"))
                {
                    return;
                }

                var command = new CreateRaceCommand();

                foreach (var data in SeedData())
                {
                    command.CreateRace(data);
                }



            }

        }

        private List<Race.Model.Race> SeedData()
        {
            var seedData = new List<Race.Model.Race>()
            {
              new Race.Model.Race()
              {
                  Name = "Human",
                  CreatedBy = "Malleus",
                  Description = @"`Humans are highly adaptable and the most common race in the world.
They come in a wide range of skin, eye and hair colours as well as different shapes and sizes."
              },
                new Race.Model.Race()
                {
                    Name = "Elf",
                    CreatedBy = "Malleus",
                    Description = @"`Elves are shorter and slimmer than humans, they are also more in tune with nature and magic.
             They have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                },
                new Race.Model.Race()
                {
                    Name = "Dark Elf",
                    CreatedBy = "Malleus",
                    Description = @"`Dark Elves are identical to their elven brethren except their skin ranges from dark pale blue to black.
             They too have an innate ability of Sneaking, Infrasion and resistance to charm spells."
                },
                new Race.Model.Race()
                {
                    Name = "Dwarves",
                    CreatedBy = "Malleus",
                    Description = @"`Dwarves are a short muscular humanoids who prefer the mountains and the underdark where they enjoy
             digging for gold. A lot of dwarves do venture out of the caves and can be found in human settlements in the local
              tavern with a mug of Ale. They are powerful Warriors and Clerics"
                },
                new Race.Model.Race()
                {
                    Name = "Mau",
                    CreatedBy = "Malleus",
                    Description = @"`Mau, Cat like humanoid race. Info coming soon"
                },
                new Race.Model.Race()
                {
                    Name = "Tlaloc",
                    CreatedBy = "Malleus",
                    Description = @"`Tlaloc, Reptilian lizard like humanoid race. Info coming soon"
                },
            };

            return seedData;
        }
    }
}
