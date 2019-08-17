using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Engine.Character.Model;
using ArchaicQuestII.Engine.Item;
using System.Data;
using ArchaicQuestII.Engine.Core.Events;
using ArchaicQuestII.Engine.Effect;
using ArchaicQuestII.Engine.Skill;
using ArchaicQuestII.Engine.Skill.Enum;
using ArchaicQuestII.Engine.Skill.Model;
using ArchaicQuestII.Engine.World.Room.Model;

namespace ArchaicQuestII.Engine.Spell.Model
{
    public class Spell:Skill.Model.Skill
    {
        public Sphere SpellGroup { get; set; }
        public SpellType Type { get; set; }
    }

}

