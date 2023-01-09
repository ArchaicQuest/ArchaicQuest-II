using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.Spell.Interface;
using ArchaicQuestII.GameLogic.Spell.Spells.DamageSpells;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class SpellsExtensions
    {
        public static IServiceCollection AddSpells(this IServiceCollection services)
        {
            services.AddSingleton<ISpellList, SpellList>();
           // services.AddSingleton<IDamageSpells, DamageSpells>();

            services.AddTransient<ISpellTargetCharacter, SpellTargetCharacter>();
           // services.AddTransient<ISpells, CastSpell>();

            return services;
        }
    }
}
