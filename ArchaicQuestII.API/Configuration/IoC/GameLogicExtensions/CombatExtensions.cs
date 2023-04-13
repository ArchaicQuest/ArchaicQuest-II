using ArchaicQuestII.GameLogic.Combat;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CombatExtensions
    {
        public static IServiceCollection AddCombatLogic(this IServiceCollection services)
        {
            services.AddSingleton<IFormulas, Formulas>();

            return services;
        }
    }
}
