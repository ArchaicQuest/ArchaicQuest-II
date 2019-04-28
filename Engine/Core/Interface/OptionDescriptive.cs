using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.Core.Interface
{
    public class Option
    {
        public int Id { get; set; }
        [BsonField("n")]
        public string Name { get; set; }
        [BsonField("dc")]
        public DateTimeOffset DateCreated { get; set; }
        [BsonField("cb")]
        public string CreatedBy { get; set; }
    }
}
