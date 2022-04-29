using ArchaicQuestII.GameLogic.Commands;
using ArchaicQuestII.GameLogic.Commands.Communication;
using ArchaicQuestII.GameLogic.Commands.Debug;
using ArchaicQuestII.GameLogic.Commands.Inventory;
using ArchaicQuestII.GameLogic.Commands.Movement;
using ArchaicQuestII.GameLogic.Commands.Objects;
using ArchaicQuestII.GameLogic.Commands.Score;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class CommandExtensions
    {
        public static IServiceCollection AddGameCommands(this IServiceCollection services)
        {
            services.AddSingleton<Icommunication, Communication>();
            services.AddSingleton<ICommands, Commands>();
            services.AddSingleton<IScore, Score>();
            services.AddSingleton<ICommandHandler, CommandHandler>();

            services.AddTransient<IMovement, Movement>();
            services.AddTransient<IDebug, Debug>();
            services.AddTransient<IInventory, Inventory>();
            services.AddTransient<IObject, Object>();

            return services;
        }
    }
}
