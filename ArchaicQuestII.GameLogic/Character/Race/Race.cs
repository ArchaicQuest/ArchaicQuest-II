using System.Collections.Generic;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using LiteDB;

namespace ArchaicQuestII.GameLogic.Character.Race
{
    public class Race : OptionDescriptive
    {
        [BsonField("p")]
        public bool Playable { get; set; }

        [BsonField("a")]
        public Attributes Attributes { get; set; } = new Attributes();

        [BsonField("s")]
        public List<SkillList> Skills { get; set; } = new List<SkillList>();
    }
}
