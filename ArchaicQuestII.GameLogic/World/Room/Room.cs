using System;
using System.Collections.Generic;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Item;

namespace ArchaicQuestII.GameLogic.World.Room
{
    public class Room
    {


        public enum TerrainType
        {
            Inside, //no weather
            City,
            Field,
            Forest,
            Hills,
            Mountain,
            Water,
            Underwater,
            Air,
            Desert,
            Underground //no weather

        }

        public enum RoomType
        {
            Standard = 0,
            Shop = 1 << 1,
            Guild = 1 << 2,
            Town = 1 << 3,
            Water = 1 << 4,
            River = 1 << 5,
            Sea = 1 << 6,
            PointOfInterest = 1 << 7,
            Field = 1 << 8,
            Forest = 1 << 9,
            Desert = 1 << 10,
            Inside = 1 << 11,
            Underground = 1 << 12
        }

        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int AreaId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// List of available exits
        /// North, East, West, South, Up, and Down
        /// </summary>
        public ExitDirections Exits { get; set; } = new ExitDirections();
        public Coordinates Coords { get; set; } = new Coordinates();
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> Mobs { get; set; } = new List<Player>();
        public ItemList Items { get; set; } = new ItemList();
        public RoomType? Type { get; set; } = RoomType.Standard;
        public TerrainType? Terrain { get; set; } = TerrainType.City;
        /// <summary>
        /// List of emotes that will be randomly played on tick
        /// </summary>
        public List<string> Emotes { get; set; } = new List<string>();
        /// <summary>
        /// Room descriptions will contain nouns which should be 
        /// extended with a keyword so a player can examine 'noun' or
        /// look 'noun' for more information about an object mentioned
        /// in the room description
        /// </summary>
        public List<RoomObject> RoomObjects { get; set; } = new List<RoomObject>();
        /// <summary>
        /// Has the room been touched or not
        /// </summary>
        public bool? Clean { get; set; } = true;
        /// <summary>
        /// When room re-populates we may want to send
        /// an emote to any players in the room
        /// </summary>
        public string UpdateMessage { get; set; }
        /// <summary>
        /// Does this repop every tick
        /// </summary>
        public bool InstantRePop { get; set; }
        public bool RoomLit { get; set; }
        public DateTime DateCreated { get; set; } = new DateTime();
        public DateTime DateUpdated { get; set; }



    }


}
