using System;

namespace ArchaicQuestII.GameLogic.Effect
{
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
}