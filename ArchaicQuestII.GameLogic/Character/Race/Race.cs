using ArchaicQuestII.GameLogic.Character.Model;
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
    }
}
