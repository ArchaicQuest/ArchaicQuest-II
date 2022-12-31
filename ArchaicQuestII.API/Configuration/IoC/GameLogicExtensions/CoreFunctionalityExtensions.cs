using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CoreFunctionalityExtensions
    {
        public static IServiceCollection AddCoreFunctionality(this IServiceCollection services)
        {
            services.AddSingleton<ICache>(new Cache());
            services.AddSingleton<IDamage, Damage>();
            services.AddSingleton<IGameLoop, GameLoop>();
            services.AddSingleton<IUpdateClientUI, UpdateClientUI>();
            services.AddSingleton<IMobScripts, MobScripts>();
            services.AddSingleton<ITime, Time>();
            services.AddSingleton<ICore, Core>();
            services.AddSingleton<IQuestLog, QuestLog>();
            services.AddSingleton<IWeather, Weather>();
            services.AddSingleton<ICommandHandler, CommandHandler>();
            services.AddSingleton<IErrorLog, ErrorLog>();

            return services;
        }
    }
}
