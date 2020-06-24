using System;

namespace ArchaicQuestII.GameLogic.Skill.Enum
{

    /***
     **  Possible Targets:
     **  TargetIgnore          : IGNORE TARGET.
     **  TargetPlayerRoom      : PC/NPC in room.
     **  TargetPlayerWorld     : PC/NPC in world.
     **  TargetFightSelf       : If fighting, and no argument, select tar_char as self.
     **  TargetFightVictim     : If fighting, and no argument, select tar_char as victim (fighting).
     **  TargetSelfOnly        : If no argument, select self, if argument check that it IS self.
     **  TargetNotSelf         : Target is anyone else besides self.
     **  TargetObjectInventory : Object in inventory.
     **  TargetObjectRoom      : Object in room.
     **  TargetObjectWorld     : Object in world.
     **  TargetObjectEquipped  : Object held.
     ***/
    [Flags]
    public enum ValidTargets
    {
        TargetIgnore = 0,
        TargetPlayerRoom = 1 << 0,
        TargetPlayerWorld = 1 << 1,
        TargetFightSelf = 1 << 2,
        TargetFightVictim = 1 << 3,
        TargetSelfOnly = 1 << 4,
        TargetNotSelf = 1 << 5,
        TargetObjectInventory = 1 << 6,
        TargetObjectRoom = 1 << 7,
        TargetObjectWorld = 1 << 8,
        TargetObjectEquipped = 1 << 9
    };
}
