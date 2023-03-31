using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ArchaicQuestII.API.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<AdminUser> WithoutPasswords(this IEnumerable<AdminUser> users)
        {
            return users?.Select(x => x.WithoutPassword());
        }

        public static AdminUser WithoutPassword(this AdminUser user)
        {
            if (user == null) return null;

            user.Password = null;
            return user;
        }

        public static void StartLoops(this IApplicationBuilder app)
        {
            var gameloop = app.ApplicationServices.GetRequiredService<IGameLoop>();

            Task.Run(TelnetHub.Instance.ProcessConnections);
            gameloop.StartLoops();
        }
    }
}
