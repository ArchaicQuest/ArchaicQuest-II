﻿using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.World.Area;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CoreFunctionalityExtensions
    {
        public static IServiceCollection AddCoreFunctionality(this IServiceCollection services)
        {
            services.AddSingleton<ICache>(new Cache());
            services.AddSingleton<IDamage, Damage>();
            services.AddSingleton<IDice, ArchaicQuestII.GameLogic.Item.Dice>();
            services.AddSingleton<IGameLoop, GameLoop>();
            services.AddSingleton<IUpdateClientUI, UpdateClientUI>();
            services.AddSingleton<IMobScripts, MobScripts>();
            services.AddSingleton<ITime, Time>();
            services.AddSingleton<ICore, GameLogic.Core.Core>();
            services.AddSingleton<IQuestLog, QuestLog>();
            services.AddSingleton<IWeather, Weather>();
            services.AddSingleton<IAreaActions, AreaActions>();

            return services;
        }
    }
}
