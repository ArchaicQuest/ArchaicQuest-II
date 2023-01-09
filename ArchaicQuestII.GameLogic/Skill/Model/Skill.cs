using System;
using ArchaicQuestII.GameLogic.Skill.Enum;
using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character.Status;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Skill.Model


{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManaCost { get; set; }
        public int MoveCost { get; set; }
    }
 
}
