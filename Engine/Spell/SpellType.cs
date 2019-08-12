using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Spell
{
    public class SpellType
    {
        public bool Affect { get; set; }
        public bool Travel { get; set; }
        public bool Creation { get; set; }
        public bool Summon { get; set; }
    }
}
