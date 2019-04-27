using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.Core.Character.Model;
using LiteDB;

namespace ArchaicQuestII.Core.Character.Race.Model
{
    public class Race
    {
 
        public int Id { get; set; }
        [BsonField("n")]
        public string Name { get; set; }
        [BsonField("d")]
        public string Description { get; set; }
        [BsonField("p")]
        public bool Playable { get; set; }
        [BsonField("a")]
        public Attributes Attributes { get; set; } = new Attributes();
    }
}
