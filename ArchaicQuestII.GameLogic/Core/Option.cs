using System;
using LiteDB;

namespace ArchaicQuestII.GameLogic.Core
{
    public class Option
    {
        public int Id { get; set; }
        [BsonField("n")]
        public string Name { get; set; }
        [BsonField("dc")]
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        [BsonField("cb")]
        public string CreatedBy { get; set; } = "Malleus";
    }
}
