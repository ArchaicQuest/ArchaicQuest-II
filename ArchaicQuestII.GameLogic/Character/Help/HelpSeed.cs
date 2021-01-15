using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Character.Help
{
    public class HelpSeed
    {
        public List<Help> SeedData()
        {
            var seedData = new List<Help>()
            {
                new Help()
                {
                    Title = "Movement",
                    Keywords = "Movement, North, East, South, West, Up, Down, Go, Walk",
                    BriefDescription = "How to get around and navigate ArchaicQuest",
                    Description = "<p>To move around you type in one of the following commands: north, east, south, west, up, down northeast, southeast, southwest, and northwest. These commands may also be shortened to:  n, e, s, w, u, d, ne, se, sw, and nw.</p>" +
                                  "<p>Moving consumes movement points, shown in the green stat bar. Stats Replenish slowly but can be sped up by using the sit, rest, or sleep commands. When finished recovering you will need to wake or stand before you can move again.</p>",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    RelatedHelpFiles = ""
                }
            };

            return seedData;
        }
    }
}
