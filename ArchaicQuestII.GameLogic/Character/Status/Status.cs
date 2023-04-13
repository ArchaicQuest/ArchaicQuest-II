using System;

namespace ArchaicQuestII.GameLogic.Character.Status
{
    public class CharacterStatus
    {
        [Flags]
        public enum Status
        {
            Standing = 0,
            Sitting = 1 << 0,
            Sleeping = 1 << 1,
            Fighting = 1 << 2,
            Resting = 1 << 3,
            Incapacitated = 1 << 4,
            Dead = 1 << 6,
            Ghost = 1 << 7,
            Busy = 1 << 8,
            Floating = 1 << 9,
            Mounted = 1 << 10,
            Stunned = 1 << 11,
            Fleeing = 1 << 12,
        }
    }
}
