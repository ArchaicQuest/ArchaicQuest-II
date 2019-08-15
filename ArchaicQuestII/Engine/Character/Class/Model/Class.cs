using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.Character.Class.Model
{
    public class Class
    {
        public int Id { get; set; }
        [BsonField("n")]
        public string Name { get; set; }
        [BsonField("d")]
        public string Description { get; set; }
    }
}
