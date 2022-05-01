using ArchaicQuestII.GameLogic.World.Room;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Configuration.IoC.GameLogicExtensions
{
    public static class WorldExtensions
    {
        public static IServiceCollection AddWorld(this IServiceCollection services)
        {
            services.AddTransient<IRoomActions, RoomActions>();
            services.AddTransient<IAddRoom, AddRoom>();

            return services;
        }
    }
}
