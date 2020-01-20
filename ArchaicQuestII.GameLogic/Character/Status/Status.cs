using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Engine.Character.Status
{   [Flags]
    public enum Status
    {
        Standing = 0,
        Sitting = 1,
        Sleeping = 2,
        Fighting = 3,
        Resting = 4,
        Incapitated = 5,
        Dead = 6,
        Ghost = 7,
        Busy = 8,
        Floating = 9,
        Mounted = 10,
        Stunned = 11

    }
}
