using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Quests
    {
        internal static void SeedAndCache()
        {
            var quests = Services.Instance.DataBase.GetList<Quest>(DataBase.Collections.Quests);

            foreach (var quest in quests)
            {
                Services.Instance.Cache.AddQuest(quest.Id, quest);
            }
        }
    }
}
