using System;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Item;
using LiteDB;
using System.Collections.Generic;
using Money = ArchaicQuestII.GameLogic.Item.Money;

namespace ArchaicQuestII.GameLogic.Character
{
    public class Character
    {
        /// <summary>
        /// Assigned when player logs in.
        /// used to find player in cached dictionary and to send data directly to player
        /// </summary>
        [BsonIgnore]
        public int ConnectionId { get; set; }
        /// <summary>
        /// Associated Account Id
        /// </summary>
        public Guid Aid { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Displays short description of the character in the room
        /// For Players this would work as a 'Pose' and for mobs
        /// will be something like 'A bat flaps around the cavern above.'
        /// </summary>
        public string LongName { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string ClassName { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public int AlignmentScore { get; set; } = 0;
        public int TotalExperience { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public Equipment.Equipment Equipped { get; set; } = new Equipment.Equipment();
        public List<Item.Item> Inventory { get; set; } = new List<Item.Item>();
        public Stats Stats { get; set; }
        public CharacterStatus.Status Status { get; set; }
        public Stats MaxStats { get; set; }
        public Attributes Attributes {get; set; }
        public Attributes MaxAttributes {get; set; }
        [BsonIgnore]
        public string Target { get; set; }
        public ArmourRating ArmorRating { get; set; }
        public Money Money { get; set; }
        public Affects Affects { get; set; }
        public Config Config { get; set; }
        
        
        

        
    }
}
