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
        public string Keyword { get; set; }
        public bool Door { get; set; } = true;
        public bool Closed { get; set; } = true;
        public bool Locked { get; set; }
        public bool PickProof { get; set; }
        public bool NoPass { get; set; }
        public bool NoScan { get; set; }
        public bool Hidden { get; set; }
        public Guid? LockId { get; set; }
    }
}
