
using ArchaicQuestII.Core.Item;
namespace ArchaicQuestII.Core.Character.Model
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public string ClassName { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public int AlignmentScore { get; set; } = 0;
        public int TotalExperience { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; }
        public Stats Stats { get; set; }
        public Stats MaxStats { get; set; }
        public Attributes Attributes {get; set; }
        public Attributes MaxAttributes {get; set; }
        public string Target { get; set; }
        public ArmourRating ArmorRating { get; set; }
        public Money Money { get; set; }
        public Affects Affects { get; set; }
        public Config Config { get; set; }
        
        
        

        
    }
}
