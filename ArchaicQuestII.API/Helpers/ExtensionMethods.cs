using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Core;
using ArchaicQuestII.GameLogic.Hubs.Telnet;
using ArchaicQuestII.GameLogic.Skill.Skills;
using ArchaicQuestII.GameLogic.Spell;
using ArchaicQuestII.GameLogic.World.Room;
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
            CoreHandler.Instance.InitServices(
                app.ApplicationServices.GetRequiredService<ICache>(),
                app.ApplicationServices.GetRequiredService<IWriteToClient>(),
                app.ApplicationServices.GetRequiredService<IDataBase>(),
                app.ApplicationServices.GetRequiredService<IUpdateClientUI>(),
                app.ApplicationServices.GetRequiredService<ICombat>(),
                app.ApplicationServices.GetRequiredService<IPlayerDataBase>(),
                app.ApplicationServices.GetRequiredService<IRoomActions>(),
                app.ApplicationServices.GetRequiredService<IMobScripts>(),
                app.ApplicationServices.GetRequiredService<IErrorLog>(),
                app.ApplicationServices.GetRequiredService<IPassiveSkills>(),
                app.ApplicationServices.GetRequiredService<IFormulas>(),
                app.ApplicationServices.GetRequiredService<ITime>(),
                app.ApplicationServices.GetRequiredService<IDamage>(),
                app.ApplicationServices.GetRequiredService<ISpellList>(),
                app.ApplicationServices.GetRequiredService<IWeather>(),
                app.ApplicationServices.GetRequiredService<ICharacterHandler>(),
                app.ApplicationServices.GetRequiredService<IGameLoop>()
                );

            Task.Run(TelnetHub.Instance.ProcessConnections);
            CoreHandler.Instance.GameLoop.StartLoops();
        }
    }
}
