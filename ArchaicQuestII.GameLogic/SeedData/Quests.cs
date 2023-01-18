using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Character;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Quests
    {
        internal static void SeedAndCache(IDataBase db, ICharacterHandler characterHandler)
        {
            var quests = db.GetList<Quest>(DataBase.Collections.Quests);

            foreach (var quest in quests)
            {
                characterHandler.AddQuest(quest.Id, quest);
            }
        }
    }
}
