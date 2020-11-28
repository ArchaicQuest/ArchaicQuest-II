using System.Collections.Generic;
using System.Linq;
using ArchaicQuestII.API.Entities;

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
    }
}
