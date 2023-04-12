using ArchaicQuestII.GameLogic.Skill.Model;

namespace ArchaicQuestII.GameLogic.Spell
{
    public class SkillMessage
    {
        public Messages NoEffect { get; set; } = new Messages();
        public Messages EffectWearOff { get; set; } = new Messages();
        public Messages Hit { get; set; } = new Messages();
        public Messages Death { get; set; } = new Messages();
        public Messages Miss { get; set; } = new Messages();
    }

    public class SavingThrow
    {
        public bool Reflex { get; set; } // check dex
        public bool Mental { get; set; } // check intelligence and wisdom
        public bool Strength { get; set; } // check strength and constitution?
    }

    public class DefineSpell
    {
        /// <summary>
        /// shows in players Affect list
        /// spells or skills can affect the player
        /// for a certain duration.
        /// example: Invis - duration (5)
        /// example Ogre Strength
        ///         Strength +10 - duration (10)
        /// </summary>
        public enum SpellAffect
        {
            Strength = 1,
            Dexterity = 2,
            Constitution = 3,
            Wisdom,
            Intelligence,
            Charisma,
            HitPoints = 6,
            Mana,
            Moves,
            HitRoll,
            DamRoll,
            Invis,
            DetectInvis,
            Blind,
            Flying,
            Floating,
            Infravision,
            Poison,
            ArmorClass,
            Curse,
            DetectAlign,
            DetectEvil,
            DetectNeutral,
            DetectGood,
            DetectSneak,
            DetectHidden,
            ProtectEvil,
            ProtectNeutral,
            ProtectGood,
            Sanctuary,
            Sleep,
            Waterwalk,
            Hidden,
            Sneak,
            NonDetect,
            Charm,
            Silence,
            Darkness,
            Dispell,
            Frozen,
            Burnt,
            Undead,
            Berserk,
            Haste
        }
    }

    public class SeedTestSpells { }
}
