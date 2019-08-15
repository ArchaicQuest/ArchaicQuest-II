using ArchaicQuestII.Engine.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Spell.Model
{
    public class LevelBasedMessages
    {
        public bool HasLevelBasedMessages { get; set; }
        public Messages Ten { get; set; }
        public Messages Twenty { get; set; }
        public Messages Thirty { get; set; }
        public Messages Forty { get; set; }
        public Messages Fifty { get; set; }



    }
}
