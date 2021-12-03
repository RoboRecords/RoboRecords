using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RoboRecords.Models;
using RoboRecords.DbInteraction;

namespace RoboRecords.Services
{
    [Flags]
    public enum UserRoles : int
    {
        None = 0,
        User = 1,
        Moderator = 2,
        Admin = 4,
    }
    public class Validator
    {

        public static string[] TrySplitUsername(string usernamewithdiscrim)
        {
            string expected = @"(.+)#(\d{4})";

            Regex rguser = new Regex(expected);

            if (usernamewithdiscrim == null || usernamewithdiscrim == "")
                return new string[] { "Invalid User", "0000" };

            Match match = rguser.Match(usernamewithdiscrim);
            if (match.Success)
            {
                return new string[] { match.Groups[1].Value, match.Groups[2].Value };
            }
            else
            {
                // TODO: Randomly generate a discriminator if not specified.
                return new string[] { usernamewithdiscrim, "0000" };
            }

        }

        public static bool UserHasRequiredRoles(IdentityRoboUser user, UserRoles roles)
        {
            if (user.Roles == 0)
                return false; // User is banned

            else if ((user.Roles & (int)roles) == (int)roles) // User has the required roles
                return true;

            return false; // User doesn't have the required roles
        }

        public static void GrantRolesToUser(IdentityRoboUser user, UserRoles roles)
        {
            user.Roles = user.Roles | (int)roles;
            DbUpdater.UpdateIdentityUser(user);
        }

        public static void RevokeRolesFromUser(IdentityRoboUser user, UserRoles roles)
        {
            user.Roles = user.Roles & (int.MaxValue ^ (int)roles);
            DbUpdater.UpdateIdentityUser(user);
        }
    }
}
