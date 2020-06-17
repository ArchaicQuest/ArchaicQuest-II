using System;
using System.Collections.Generic;
using System.Text;

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

    public class EffectModifier
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

    [Flags]
    public enum EffectFlags
    {
        None = 0,
        Blind = 1 << 0, //Player can't see
        Deaf = 1 << 1, //Player can't hear or cast spells
        Cursed = 1 << 2,
        Silenced = 1 << 3,
        Sanctuary = 1 << 4,
        Poison = 1 << 5,
        ProtectEvil = 1 << 6,
        ProtectNeutral = 1 << 7,
        ProtectGood = 1 << 8,
        Sleep = 1 << 9,
        Hide = 1 << 10,
        Sneak = 1 << 11,
        Charm = 1 << 12,
        Infrared = 1 << 13,
        DetectInvisible = 1 << 14,
        NonDetect = 1 << 15,
        DetectEvil = 1 << 16,
        DetectNeutral = 1 << 16,
        DetectGood = 1 << 16,
    }

    public enum EffectExpression
    {
        None = 0,
        Addition = 1 << 0,
        Divide = 1 << 1,
        Equal = 1 << 2,
        Multiply = 1 << 3,
        Substract = 1 << 4,

    }

    public enum EffectLocation
    {
        None = 0,
        Strength = 1 << 0,
        Dexterity = 1 << 1,
        Constitution = 1 << 2,
        Intelligence = 1 << 3,
        Wisdom = 1 << 4,
        Charisma = 1 << 5,
        Luck = 1 << 6,
        Hitpoints = 1 << 7,
        Mana = 1 << 8,
        Moves = 1 << 9,
        Armour = 1 << 10,
        HitRoll = 1 << 11,
        SavingSpell = 1 << 12,
        DamageRoll = 1 << 13,
        Gender = 1 << 14,
        Age = 1 << 15,
        Weight = 1 << 16,
        Height = 1 << 16,
        Level = 1 << 16,
    }
}