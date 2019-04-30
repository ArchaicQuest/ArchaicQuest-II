using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace ArchaicQuestII.Engine.Core.Interface
{
    public class OptionDescriptive : Option
    {
        [BsonField("d")]
        public string Description { get; set; }
    }
}
