using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Core;

namespace ArchaicQuestII.GameLogic.SeedData
{
    internal static class ConfigOnInit
    {
        public static void SeedAndCache()
        {
            if (!Services.Instance.DataBase.DoesCollectionExist(DataBase.Collections.Config))
            {
                Services.Instance.DataBase.Save(new Config(), DataBase.Collections.Config);
            }

            var config = Services.Instance.DataBase.GetById<Config>(1, DataBase.Collections.Config);

            if (config != null)
                Services.Instance.Config = config;
        }
    }
}
