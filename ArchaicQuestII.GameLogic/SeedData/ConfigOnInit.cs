using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class ConfigOnInit
    {
        public static void SeedAndCache(ICoreHandler coreHandler)
        {
            if (!coreHandler.Db.DoesCollectionExist(DataBase.Collections.Config))
            {
                coreHandler.Db.Save(new Config(), DataBase.Collections.Config);
            }

            var config = coreHandler.Db.GetById<Config>(1, DataBase.Collections.Config);
            coreHandler.Config = config;
        }
    }
}
