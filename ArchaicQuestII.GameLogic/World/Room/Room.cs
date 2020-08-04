using System;
using System.Collections.Generic;
using ArchaicQuestII.Core.World;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;

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
            Standard,
            Shop,
            Guild,
            Town
        }

        public int Id { get; set; }
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
        public List<Item.Item> Items { get; set; } = new List<Item.Item>();
        public RoomType? Type { get; set; } = RoomType.Standard;
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
        public DateTime DateCreated { get; set; } = new DateTime();
        public DateTime DateUpdated { get; set; }


 
    }

    
}
