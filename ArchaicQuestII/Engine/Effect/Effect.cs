using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Effect
{
    public class Effect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// How long the affect lasts
        /// </summary>
        public EffectModifer Duration { get; set; }
        /// <summary>
        /// How much modifier to apply to the affect location
        /// can be positive or negative
        /// e.g Modify Strength by -5
        /// </summary>
        public EffectModifer Modifier { get; set; }
        /// <summary>
        /// Does the effect stack?
        /// </summary>
        public bool Accumulate { get; set; }
        /// <summary>
        /// What is affected
        /// </summary>
        public EffectLocation Location { get; set; }
    }

    public class EffectModifer
    {
        public int Value { get; set; }
        public bool PositiveEffect { get; set; }
      
    }


    public class EffectChecks
    {
        public bool LevelCheck { get; set; }
        public bool GoodCheck { get; set; }
        public bool EvilCheck { get; set; }

    }

    public enum EffectExpression
    {
            None = 0,
            Addition = 1 << 0,
            Divide = 1 << 1,
            Equal  = 1 << 2,
            Multiply = 1 << 3,
            Substract = 1 << 4,

    }
}
