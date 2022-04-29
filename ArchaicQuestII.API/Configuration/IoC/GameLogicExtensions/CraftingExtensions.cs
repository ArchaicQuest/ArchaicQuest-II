using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Healer;
using ArchaicQuestII.GameLogic.Crafting;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CraftingExtensions
    {
        public static IServiceCollection AddCrafting(this IServiceCollection services)
        {
            services.AddSingleton<ICrafting, Crafting>();
            services.AddSingleton<ICooking, Cooking>();

            return services;
        }
    }
}
