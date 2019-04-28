using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArchaicQuestII.Core.Item
{
    public class BaseItem
    {
        public Guid? Uuid { get; set; }
        public int Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        /// <summary>
        /// Min level needed to use item
        /// </summary>
        public int Level { get; set; }
        public bool KnownByName { get; set; }
        public Description Description { get; set; }
        /// <summary>
        /// Alternative names to target item
        /// armor / armour as an example
        /// </summary>
        public List<string> Keywords { get; set; }
        /// <summary>
        /// isHiddenInRoom is for objects that are mentioned in
        /// the descriptions but do not need to be shown in the
        /// room item list and for items that you do not want to be discovered
        /// by other means
        /// </summary>
        public bool isHiddenInRoom { get; set; }
        /// <summary>
        /// Hidden items are not visible
        /// unless player has means to see them
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// Cannont remove item
        /// </summary>
        public bool Stuck { get; set; }
        /// <summary>
        /// how many ticks till item decays
        /// 0 is never
        /// </summary>
        public int DecayTimer { get; set; } = 0;
        public int Condition { get; set; } = 5;// Helpers.Rand(75, 100);
        public bool QuestItem { get; set; }
    }
}
