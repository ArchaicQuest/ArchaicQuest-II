using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character.Model;
using ArchaicQuestII.GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class Quests
    {
        internal static void SeedAndCache(IDataBase db, ICache cache)
        {
            var quests = db.GetList<Quest>(DataBase.Collections.Quests);

            foreach (var quest in quests)
            {
                cache.AddQuest(quest.Id, quest);
            }
        }
    }
}
