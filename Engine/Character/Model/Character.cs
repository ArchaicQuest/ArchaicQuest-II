
using ArchaicQuestII.Engine.Item;
using System.Collections.Generic;
using ArchaicQuestII.Engine.Character.Gender;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Model
{
    public class Character
    {
        public int Id { get; set; }
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
        public Equipment.Model.Equipment Equipment { get; set; } = new Equipment.Model.Equipment();
        public List<Item.Item> Inventory { get; set; } = new List<Item.Item>();
        public Stats Stats { get; set; }
        public Status.Status Status { get; set; }
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
