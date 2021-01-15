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
                    Description = "<p>To move around you type in one of the following commands: <span class='hint'>north</span>, <span class='hint'>east</span>, <span class='hint'>south</span>, <span class='hint'>west</span>, <span class='hint'>up</span>, <span class='hint'>down</span> <span class='hint'>northeast</span>, <span class='hint'>southeast</span>, <span class='hint'>southwest</span>, and <span class='hint'>northwest</span>. These commands may also be shortened to:  <span class='hint'>n</span>, <span class='hint'>e</span>, <span class='hint'>s</span>, <span class='hint'>w</span>, <span class='hint'>u</span>, <span class='hint'>d</span>, <span class='hint'>ne</span>, <span class='hint'>se</span>, <span class='hint'>sw</span>, and <span class='hint'>nw</span>.</p><p>Moving consumes movement points, shown in the green stat bar. Stats Replenish slowly but can be sped up by using the sit, rest, or sleep commands. When finished recovering you will need to wake or stand before you can move again.</p>",
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    RelatedHelpFiles = ""
                },
                new Help()
                {
                    Title = "Look",
                    Keywords = "Look, room, display",
                    BriefDescription = "How to view room and objects",
                    Description = "Typing <span class='hint'>look</span> or <span class='hint'>l</span> will display the current room you are in. Look can take these parameters: <ul><li>look &lt;thing&gt;</li><li>look in &lt;container&gt;</li></ul> To look at objects in the room such as players, mobs, items or objects mentioned in the description type look &lt;thing&gt; for example: look sword.\r\n\r\nTo look inside a container type look in  &lt;container&gt; example: look in bag.\r\n\r\nFor a detailed look at objects with more verbose descriptions see <span class='hint'>Help Examine</span>\r\n\r\n",

                     DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    RelatedHelpFiles = "Examine"
                },
                new Help()
                {
                    Title = "Examine",
                    Keywords = "examine, room, display",
                    BriefDescription = "How to view detailed descriptions of objects",
                    Description = "Typing <span class='hint'>examine &lt;thing&gt;</span> or <span class='hint'>exam &lt;thing&gt;</span> will display detailed descriptions if they exist on a object. If none exists it defaults to the basic look description.\r\n\r\n Usage: <span class='hint'>examine sword</span>",

                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    RelatedHelpFiles = "Look"
                },
                new Help()
                {
                    Title = "Get",
                    Keywords = "get, pick, grab",
                    BriefDescription = "How to get items",
                    Description = "Typing <span class='hint'>get &lt;thing&gt;</span> will pick up the item in the room.\r\n\r\n Usage: <span class='hint'>get sword</span>. \r\n\r\n To see the item you have just picked up type <span class='hint'>inventory</span> or <span class='hint'>i</span> for short to display the items you are currently carry. \r\n\r\n To drop an item type <span class='hint'>drop &lt;thing&gt;</span> e.g. drop sword. This item will now be available in the room for others to pick up.",

                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    RelatedHelpFiles = "Drop, Inventory"
                }
            };

            return seedData;
        }
    }
}
