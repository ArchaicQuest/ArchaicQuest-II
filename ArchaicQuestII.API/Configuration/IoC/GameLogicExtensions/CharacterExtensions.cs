using ArchaicQuestII.GameLogic.Character.Equipment;
using ArchaicQuestII.GameLogic.Character.Help;
using ArchaicQuestII.GameLogic.Character.MobFunctions.Healer;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CharacterExtensions
    {
        public static IServiceCollection AddCharacterLogic(this IServiceCollection services)
        {
            services.AddSingleton<IHealer, Healer>();
            services.AddSingleton<IHelp, HelpFile>();

            services.AddTransient<IEquip, Equip>();

            return services;
        }
    }
}
