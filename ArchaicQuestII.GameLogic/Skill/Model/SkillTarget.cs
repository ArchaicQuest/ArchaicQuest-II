using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.World.Room;

namespace ArchaicQuestII.GameLogic.Skill.Model
{
    public class SkillTarget
    {
        public Skill Skill { get; set; }
        public Player Origin { get; set; }
        public Player Target { get; set; }
        public Room Room { get; set; }
 
    }
}
