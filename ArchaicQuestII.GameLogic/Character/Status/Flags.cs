using System;

namespace ArchaicQuestII.GameLogic.Character.Status
{
    [Flags]
    public enum CharacterFlags
    {
        NonPlayer = 0,
        Player = 1 << 0,
        Aggressive = 1 << 1,
        Shopkeeper = 1 << 2,
        Trainer = 1 << 3,
        Pet = 1 << 4,
        Mount = 1 << 6,
    }
}