using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;

namespace ArchaicQuestII.Core.Room
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
        /// <summary>
        /// A Mud can have many areas, to organise these
        /// they will be grouped under regions
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// An area is part of the game world, there is no limit
        /// to how many rooms you have in an area
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// AreaId is the Unique identifier of the room
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// Used for mapping, can use this type to style 
        /// the colour of nodes on a map or emote global
        /// text based on the type
        /// </summary>
        public RoomType Type { get; set; } = RoomType.Standard;
        /// <summary>
        /// Has player visited this room? 
        /// if not award XP
        /// </summary>
        public bool Visited { get; set; }
        /// <summary>
        /// Coordinates used for mapping
        /// </summary>
        public Coordinates Coords { get; set; } = new Coordinates();
        /// <summary>
        /// Has the room been touched or not
        /// </summary>
        public bool Clean { get; set; } = true;
        /// <summary>
        /// Last updated
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// Room Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of room
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// When room re-populates we may want to send
        /// an emote to any players in the room
        /// </summary>
        public string UpdateMessage { get; set; }
        /// <summary>
        /// If the terrain is water you will need a boat for example
        /// This is used to determine how player interacts within the room
        /// </summary>
        public TerrainType Terrain { get; set; } = TerrainType.City;
        /// <summary>
        /// Does this repop every tick
        /// </summary>
        public bool InstantRePop { get; set; }
        /// <summary>
        /// Room descriptions will contain nouns which should be 
        /// extended with a keyword so a player can examine 'noun' or
        /// look 'noun' for more information about an object mentioned
        /// in the room description
        /// </summary>
        public List<RoomObject> Keywords { get; set; }
        /// <summary>
        /// List of available exits
        /// North, East, West, South, Up, and Down
        /// </summary>
        public List<Exit> Exits { get; set; }
        /// <summary>
        /// Current players in the room
        /// </summary>
        public List<Player> Players { get; set; }
        /// <summary>
        /// Mobs in the room
        /// </summary>
        public List<Player> Mobs { get; set; }
        /// <summary>
        /// Items in the room
        /// </summary>
        public List<string> Items { get; set; }
        /// <summary>
        /// List of emotes that will be randomly played on tick
        /// </summary>
        public List<string> Emotes { get; set; } = new List<string>();
        /// <summary>
        /// Name of script to run on Enter
        /// </summary>
        public string EventOnEnter { get; set; }
        /// <summary>
        /// Name of script to run on Wake
        /// </summary>
        public string EventWake { get; set; }
        /// <summary>
        /// Name of script to run on Wear
        /// </summary>
        public string EventWear { get; set; }
        /// <summary>
        /// Name of script to run on Death
        /// </summary>
        public string EventDeath { get; set; }
        /// <summary>
        /// Name of script to run on Look
        /// </summary>
        public string EventLook { get; set; }
    }
}
