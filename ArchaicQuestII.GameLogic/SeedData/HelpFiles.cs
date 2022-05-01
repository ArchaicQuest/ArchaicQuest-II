using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class HelpFiles
    {
        private static readonly List<Help> seedData = new List<Help>()
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
                Title = "Object",
                Keywords = "Drop, Get, Give, Put, Take",
                BriefDescription = "Information about object manipulation.",
                Description = "\r\n<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>drop <span style='color:#fff'>&lt;object&gt;</span> </td><td>Drops the item from your inventory.</td></tr>\r\n<tr><td>drop <span style='color:#fff'>&lt;amount&gt;</span> coins</td><td>Drops this amount of gold if you have it.</td></tr>\r\n<tr><td>get <span style='color:#fff'>&lt;object&gt;</span>  </td><td>Picks up an item from the room.</td></tr>\r\n<tr><td>get <span style='color:#fff'>&lt;object&gt;</span> <span style='color:#fff'>&lt;container&gt;</span> </td><td>Gets an item from a container.</td></tr>\r\n<tr><td>give <span style='color:#fff'>&lt;object&gt;</span> <span style='color:#fff'>&lt;character&gt;</span></td><td>Gives an item to another.</td></tr>\r\n<tr><td>give <span style='color:#fff'>&lt;amount&gt;</span> gold <span style='color:#fff'>&lt;character&gt;</span></td><td>Gives a character gold.</td></tr>\r\n<tr><td>put <span style='color:#fff'>&lt;object&gt;</span> <span style='color:#fff'>&lt;container&gt;</span></td><td>Puts an item into a container.</td></tr>\r\n</table>\r\n'<span class='hint'>drop</span>' is used to drop a specified item or gold to the ground if you possess it. Only items in your inventory can be dropped, to drop a worn item you must remove it first.\r\n\r\nUsage: drop sword\r\n           drop all \r\n\r\n'<span class='hint'>get</span>' is used to get the specified item or gold from the ground,container, or a corpse.  \r\n\r\nUsage: get sword \r\n           get all \r\n           get all corpse \r\n\r\n'<span class='hint'>give</span>' is used to give the specified item or gold to another player or NPC.\r\n\r\nUsage: give sword Luca \r\n           give 100 gold Hugo\r\n\r\n'<span class='hint'>put</span>' is used to put the specified item into a container\r\n\r\nUsage: put sword Chest \r\n           put all chest \r\n\r\nOther commands that deal with objects are:\r\n\r\n<table class=\"simple\">\r\n<tr><td style='width:72px'>wear<td><td>wear a piece of armour.<td></tr>\r\n<tr><td style='width:72px'>wield<td><td>wield a weapon.<td></tr>\r\n<tr><td style='width:72px'>hold<td><td>hold an item<td></tr>\r\n<tr><td style='width:72px'>remove<td><td>stop using a worn, held, or wielded item.<td></tr>\r\n</table>",


            DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = "Equipment"
            },
            new Help()
            {
                Title = "Wear",
                Keywords = "wear, wearable",
                BriefDescription = "Allows a player to wear items in their inventory.  ",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>wear <span style='color:#fff'>&lt;object&gt;</span> </td><td>Wears a particular object.</td></tr>\r\n<tr><td>wear <span style='color:#fff'>all</span> coins</td><td>Attempts to wear all items in inventory.</td></tr>\r\n<tr><td>eq</td><td>Shows worn items</td></tr>\r\n</table>\r\n'<span class='hint'>wear</span>' is used to wear a particular item. This is the only way to gain the benefits of an item. Some wear locations are duplicated such as neck or wrist. To find out the item properties of an item use the identify spell or seek a mage from the guild. If you are already wearing an item in an occupied slot it will be replaced with the new item you want to wear.\r\n\r\nWear all will try to wear all items in your inventory until your equipment slots are full.\r\n\r\nUsage: wear helm\r\n           wear all \r\n\r\nOther commands that deal with items are:\r\n\r\n<table class=\"simple\">\r\n<tr><td style='width:72px'>wield<td><td>wield a weapon.<td></tr>\r\n<tr><td style='width:72px'>hold<td><td>hold an item<td></tr>\r\n<tr><td style='width:72px'>remove<td><td>stop using a worn, held, or wielded item.<td></tr>\r\n</table>",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = "Wield, hold"
            },
            new Help()
            {
                Title = "Wield",
                Keywords = "wield, weapons",
                BriefDescription = "Allows a player to wield a weapon",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>Wield<span style='color:#fff'>&lt;object&gt;</span> </td><td>Wields a weapon.</td></tr>\r\n</table>\r\n'<span class='hint'>wield</span>' is used to wield a weapon. You may not wield a weapon if it's a higher level than you or does not match your alignment or is to heavy for you to wield.\r\n\r\nUsage: wield dagger\r\n\r\nOther commands that deal with items are:\r\n\r\n<table class=\"simple\">\r\n<tr><td style='width:72px'>wear<td><td>wear equipment.<td></tr>\r\n<tr><td style='width:72px'>hold<td><td>hold an item<td></tr>\r\n<tr><td style='width:72px'>remove<td><td>stop using a worn, held, or wielded item.<td></tr>\r\n</table>",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = "Wear"
            },
            new Help()
            {
                Title = "Help",
                Keywords = "help, commands",
                BriefDescription = "Overview of commands",
                Description = "<table class='simple heading'>\n<tbody>\n<tr>\n<td>Movement</td>\n <td>Objects</td>\n</tr>  <tr>\n     <td>north south east west up down        \nnortheast northwest southeast\nsouthwest exits recall\nsleep wake rest stand</td>\n <td>get put drop give sacrifice          \nwear wield hold                     \nrecite quaff zap brandish            \nlock unlock open close pick          \ninventory equipment look compare    \neat drink fill                      \nlist buy sell value</td>\n</tr>\n  <tr>\n     <td>Combat</td>\n <td>Group</td>\n</tr>\n <tr>\n     <td>kill cast skills spells\ndodge parry wimpy flee  \nwands scrolls staves\ndamage death healers\nnosummon PK</td>\n <td> group follow nofollow gtell</td>\n</tr>  <tr>\n     <td>Character</td>\n <td>Communication</td>\n</tr>\n  <tr>\n     <td>description title \nscore report practice train stats\ncommands socials pose emote RP </td>\n <td>ic ooc newbie gossip yell shout   \nnote idea history change  \nsay tell reply who    \n   </td>\n</tr>\n</tbody>\n</table>\n\nFor more help, type 'help <topic>' for any command, skill, or spell.",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = ""
            },
            new Help()
            {
                Title = "Score",
                Keywords = "score, status",
                BriefDescription = "See detailed Character stats",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>score</td><td>Displays Character stats</td></tr>\r\n</table>\r\n'<span class='hint'>score</span>'  shows you detailed statistics about your character.\r\nAs you equip / remove items it affects your Armour Class and can be seen when viewing score.\r\nAlso, your attributes show your current value and the max value in brackets",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = "Affects"
            },
            new Help()
            {
                Title = "Affects",
                Keywords = "affects effects",
                BriefDescription = "shows all spells and skills that affect your character",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>affects</td><td>shows all spells and skills affecting you.</td></tr>\r\n</table>\r\n'<span class='hint'>affects</span>'  shows all spells and skills that affect your character\r\nThese affects may have been placed on you by yourself or other sources.  Affects will also\r\nshow the duration of affects.",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = "Score"
            },
            new Help()
            {
                Title = "Recall",
                Keywords = "recall",
                BriefDescription = "Return back to temple or clan hall",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>recall</td><td>Returns you to your recall point</td></tr>\r\n</table>\r\n'<span class='hint'>recall</span>'  using this command will teleport your character\r\nto your starting point or clan hall. It costs half of your moves each time you recall\r\nyou can use '/' as an alias.",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = ""
            },
            new Help()
            {
                Title = "Questlog",
                Keywords = "questlog, quest",
                BriefDescription = "displays current quests",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>questlog</td><td>displays current quests</td></tr>\r\n</table>\r\n'<span class='hint'>questlog</span>'  Displays your current active quests, the title, a description of the quest and the requirements required to complete the test and what you will be rewarded.",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = ""
            },
            new Help()
            {
                Title = "Train",
                Keywords = "train, trainer",
                BriefDescription = "training stats",
                Description = "<table class='simple'>\r\n<tr><td><span style='color:#fff'>Syntax</span></td><td>&nbsp;</td></tr>\r\n<tr><td>train</td><td>displays remaing trains to use</td></tr>  \r\n<tr><td>train &lt;stat&gt;</td><td>will train this stat once</td></tr>\r\n</table>\r\n'<span class='hint'>train</span>'  Displays your remaining trains which you can use to increase your stats. The Maximium a core attribute can be trained to is 100. No limits are set on HP, Mana, or moves. You can use the train command in the presence of a trainer or guild master",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                RelatedHelpFiles = ""
            }
        };

        internal static void SeedAndCache(IDataBase db, ICache cache)
        {
            if (!db.DoesCollectionExist(DataBase.Collections.Help))
            {
                foreach (var helpSeed in seedData)
                {
                    db.Save(helpSeed, DataBase.Collections.Help);
                }
            }
            else
            {
                var helpList = db.GetList<Help>(DataBase.Collections.Help);

                foreach (var helpSeed in seedData)
                {
                    if (helpList.Any(x => x.Title.Equals(helpSeed.Title)))
                    {
                        continue;
                    }

                    db.Save(helpSeed, DataBase.Collections.Help);
                }
            }

            var helpFiles = db.GetList<Help>(DataBase.Collections.Help);
            foreach (var helpFile in helpFiles)
            {
                cache.AddHelp(helpFile.Id, helpFile);
            }
        }
    }
}
