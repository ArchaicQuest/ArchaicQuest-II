﻿using ArchaicQuestII.GameLogic.Skill;
using ArchaicQuestII.GameLogic.Skill.Core;
using ArchaicQuestII.GameLogic.Skill.Skills;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class SkillsExtensions
    {
        public static IServiceCollection AddSkills(this IServiceCollection services)
        {
            services.AddSingleton<ISkillManager, SkillManager>();
            services.AddSingleton<IDamageSkills, DamageSkills>();
            services.AddSingleton<IPassiveSkills, PassiveSkills>();
            services.AddSingleton<IUtilSkills, UtilSkills>();
            services.AddSingleton<ISkillList, SkillList>();
            services.AddSingleton<ISKill, DoSkill>();
            
            return services;
        }
    }
}
