using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.World.Room.Model;

namespace ArchaicQuestII.Engine.Skill.Model
{
    public class SkillTarget
    {
        public Skill Skill { get; set; }
        public Player Origin { get; set; }
        public Player Target { get; set; }
        public Room Room { get; set; }
 
    }
}
