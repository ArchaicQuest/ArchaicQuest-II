using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchaicQuestII.API.Entities;
using ArchaicQuestII.DataAccess;
using ArchaicQuestII.GameLogic.Character;
using ArchaicQuestII.GameLogic.Client;
using ArchaicQuestII.GameLogic.Combat;
using ArchaicQuestII.GameLogic.Commands;
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
            if (user == null)
                return null;

            user.Password = null;
            return user;
        }

        public static void StartLoops(this IApplicationBuilder app)
        {
            Task.Run(TelnetHub.Instance.ProcessConnections);
            GameLogic.Core.Services.Instance.GameLoop.StartLoops();
        }
    }
}
