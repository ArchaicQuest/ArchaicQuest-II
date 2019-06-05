using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Core.World
{
    public class Exit
    {
        public Coordinates Coords { get; set; }
        public int AreaId { get; set; }
        public string Name { get; set; }
        public bool Open { get; set; } = true;
        public bool CanOpen { get; set; } = true;
        public bool? CanLock { get; set; }
        public bool? Locked { get; set; }
        public Guid? LockId { get; set; }
    }
}
