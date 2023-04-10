using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.World.Area;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class InitialRoomSeed
    {
        private static readonly Area initialArea = new Area()
        {
            Title = "Example Area",
            Description =
                "An area contains rooms, there is no limit to how many rooms you can have but for map performance I would limit to a maximum of 200-300",
            CreatedBy = "Malleus"
        };

        private static readonly List<Room> seedData = new List<Room>()
        {
            new Room()
            {
                Title = "Welcome to the dungeon",
                Description =
                    "You're in a dark dingy room lit only by the flicker of torch light. "
                    + "A worn wooden table sits crooked in the middle with a note that you should probably look at."
                    + " hint: enter <span class='room-exits'>look note</span>",
                RoomObjects = new List<RoomObject>()
                {
                    new RoomObject()
                    {
                        Name = "wooden table",
                        Look =
                            "A dilapidated wooden table with wonky legs sits crooked in the room. There is a note upon it that you should read.",
                    }
                },
                Emotes = new List<string>()
                {
                    "You hear the squeak of a mouse",
                    "The torch light flickers casting eerie shadows",
                    "A disembodied voice says 'Malleus here, Hope you make a great game, but eh could also help me on ArchaicQuest'"
                },
                Coords =
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                },
                AreaId = 1,
                IsLit = true,
                Items = new ItemList()
                {
                    new Item.Item()
                    {
                        Name = "An important note",
                        Container = new Container(),
                        Description = new Description()
                        {
                            Room = "A note has been left on the table",
                            Look =
                                "Thanks for trying out ArchaicQuest."
                                + "<p>This is the initial room, you can expand it, delete it, do what you like by using the admin tool "
                                + "to modify the game content, the initial starting room id can be changed from the settings option if you don't want to delete this.</p>"
                                + "<p>This room is just a placeholder to show you examples of what you can do. This note is a room item, you can also examine, taste, touch, and smell it!</p>"
                                + "<p><a class='room-exits' href='https://github.com/ArchaicQuest/ArchaicQuest-II/wiki/Getting-started' target='_blank'>Getting started Wiki</a> <br />"
                                + "Discord invite <a class='room-exits'  href='https://discord.gg/nuf7FVq'>https://discord.gg/nuf7FVq</a> Say hi, ask any questions you like.</p>",
                            Smell = "The note smells damp and musky",
                            Touch =
                                "The note feels slightly wet, probably caused by the occasional drip from above",
                            Taste = "Tastes putrid, what did you expect?"
                        },
                        Slot = Equipment.EqSlot.Held,
                    }
                }
            }
        };

        internal static void Seed()
        {
            if (!Services.Instance.DataBase.DoesCollectionExist(DataBase.Collections.Area))
            {
                Services.Instance.DataBase.Save(initialArea, DataBase.Collections.Area);
            }

            if (!Services.Instance.DataBase.DoesCollectionExist(DataBase.Collections.Room))
            {
                foreach (var roomSeed in seedData)
                {
                    Services.Instance.DataBase.Save(roomSeed, DataBase.Collections.Room);
                }
            }
        }
    }
}
