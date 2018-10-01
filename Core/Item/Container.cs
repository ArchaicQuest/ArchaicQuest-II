using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public bool IsOpen { get; set; }
        public bool IsLocked { get; set; }
        public Guid? AssociatedKeyId { get; set; }

    }
}
