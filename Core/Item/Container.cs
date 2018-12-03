using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ArchaicQuestII.Core.Item.Item;

namespace ArchaicQuestII.Core.Item
{
    public class Container
    {
        public enum ContainerSize
        {
            ExtraSmall,
            Small,
            Medium,
            Large,
            ExtraLarge,
            Infinite
        }

        public ContainerSize Size { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
        public bool CanOpen { get; set; }
        public bool IsOpen { get; set; }
        public bool CanLock { get; set; }
        public bool IsLocked { get; set; }
        public int GoldPieces { get; set; }
        // needs to match Key lock guid
        public Guid? AssociatedKeyId { get; set; }
        public LockStrength LockDifficulty { get; set; }

    }
}
