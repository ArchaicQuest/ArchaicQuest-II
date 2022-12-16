namespace ArchaicQuestII.GameLogic.Effect
{
    public class Effect
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// How long the affect lasts
        /// </summary>
        public EffectModifier Duration { get; set; }
        /// <summary>
        /// How much modifier to apply to the affect location
        /// can be positive or negative
        /// e.g Modify Strength by -5
        /// </summary>
        public EffectModifier Modifier { get; set; }
        /// <summary>
        /// Does the effect stack?
        /// </summary>
        public bool Accumulate { get; set; }
        /// <summary>
        /// What is affected
        /// </summary>
        public EffectLocation Location { get; set; }
    }
}