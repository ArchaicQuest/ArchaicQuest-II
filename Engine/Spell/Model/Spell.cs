using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skills;
using ArchaicQuestII.Engine.World.Room.Model;

namespace ArchaicQuestII.Engine.Spell.Model
{
    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dice Damage { get; set; }
        public Effect.Effect Effect { get; set; }
        public Requirements Requirements { get; set; }
        public Sphere SpellGroup { get; set; }
        public Messages SkillStart { get; set; }
        public List<Messages> SkillAction { get; set; }
        public Messages SkillEnd { get; set; }
        public Messages SkillFailure { get; set; }
        public LevelBasedMessages LevelBasedMessages { get; set; }
        public SpellType Type { get; set; }
        public int Rounds { get; set; }
        public Cost Cost { get; set; }
  

    }

}

