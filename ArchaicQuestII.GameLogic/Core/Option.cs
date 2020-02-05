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
        public DateTimeOffset DateCreated { get; set; } = new DateTimeOffset();
        [BsonField("cb")]
        public string CreatedBy { get; set; } = "Malleus";
    }
}
