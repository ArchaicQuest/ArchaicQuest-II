using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class ConfigOnInit
    {
        public static void SeedAndCache(IDataBase db, ICache cache)
        {
            if (!db.DoesCollectionExist(DataBase.Collections.Config))
            {
                db.Save(new Config(), DataBase.Collections.Config);
            }

            var config = db.GetById<Config>(1, DataBase.Collections.Config);
            cache.SetConfig(config);
        }
    }
}
