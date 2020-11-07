using System.Collections.Generic;

namespace ArchaicQuestII.GameLogic.Character.Model
{

    public enum QuestTypes
    {
        Kill,
        Fetch,
        Discover,
        Escort
    }

    public class Quest
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public QuestTypes Type { get; set; }
        public string Description { get; set; }
        public string Area { get; set; }
        public List<string> Rewards { get; set; }
        public bool Completed { get; set; }
    }
}
