﻿using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class GameLogicExtensions
    {
        public static IServiceCollection AddGameLogic(this IServiceCollection services)
        {
            services
                .AddCoreFunctionality()
                .AddCharacterLogic()
                .AddCombatLogic()
                .AddItems()
                .AddSkills();

            return services;
        }
    }
}
