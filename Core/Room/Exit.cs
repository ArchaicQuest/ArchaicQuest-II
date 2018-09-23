using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchaicQuestII.Core.Room
{
    public class Exit
    {
        public int AreaId { get; set; }
        public string Area { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public bool Open { get; set; } = true;
        public bool CanOpen { get; set; }
        public bool CanLock { get; set; }
        public bool Locked { get; set; }
    }
}
