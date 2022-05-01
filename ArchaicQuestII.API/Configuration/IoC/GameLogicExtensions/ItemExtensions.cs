using ArchaicQuestII.GameLogic.Item;
using ArchaicQuestII.GameLogic.Item.RandomItemTypes;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class ItemExtensions
    {
        public static IServiceCollection AddItems(this IServiceCollection services)
        {
            services.AddSingleton<IRandomWeapon, RandomWeapons>();
            services.AddSingleton<IRandomItem, RandomItem>();
            services.AddSingleton<IRandomClothItems, RandomClothItems>();
            services.AddSingleton<IRandomLeatherItems, RandomLeatherItems>();
            services.AddSingleton<IRandomStuddedLeatherArmour, RandomStuddedLeatherItems>();
            services.AddSingleton<IRandomChainMailArmour, RandomChainMailItems>();
            services.AddSingleton<IRandomPlateMailArmour, RandomPlateMailItems>();

            return services;
        }
    }
}
