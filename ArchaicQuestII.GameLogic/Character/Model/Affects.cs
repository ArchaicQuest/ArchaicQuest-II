using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Spell;

namespace ArchaicQuestII.GameLogic.Character.Model
{
    public class Affects
    {
        public bool NonDectect { get; set; }
        
        public bool Stunned { get; set; }
        
        public bool Invis { get; set; }
        
        public bool DetectInvis { get; set; }
        
        public bool Hidden { get; set; }
        
        public bool DetectHidden { get; set; }
        
        public bool Poisoned { get; set; }
        
        public bool Blind { get; set; }
        
        public bool Haste { get; set; }
        
        public bool Berserk { get; set; }
        
        public List<Affect> Custom { get; set; } = new();
        
        public bool DarkVision { get; set; }

    }

    public class Affect
    {
        public string Name { get; set; }
        public string Benefits { get; set; }
        public Modifier Modifier { get; set; }
        public int Duration { get; set; }
        public DefineSpell.SpellAffect Affects { get; set; }
    }

}
