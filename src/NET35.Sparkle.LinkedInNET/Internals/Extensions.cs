
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.LinkedInNET.OAuth2;

    internal static class Extensions
    {
        internal static string GetAuthorizationName(this AuthorizationScope scope)
        {
            switch (scope)
            {
                case AuthorizationScope.ReadBasicProfile:
                    return "r_basicprofile";
                case AuthorizationScope.ReadFullProfile:
                    return "r_fullprofile";
                case AuthorizationScope.ReadEmailAddress:
                    return "r_emailaddress";
                case AuthorizationScope.ReadNetwork:
                    return "r_network";
                case AuthorizationScope.ReadContactInfo:
                    return "r_contactinfo";
                case AuthorizationScope.ReadWriteNetworkUpdates:
                    return "rw_nus";
                case AuthorizationScope.ReadWriteCompanyPage:
                    return "rw_company_admin";
                case AuthorizationScope.ReadWriteGroups:
                    return "rw_groups";
                case AuthorizationScope.WriteMessages:
                    return "w_messages";
                default:
                    throw new NotSupportedException("Scope of value '" + scope.ToString() + " is not supported");
            }
        }

        internal static long ToUnixTime(this DateTime value)
        {
            var unix = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            if (value < unix)
                throw new ArgumentException("Specified value is lower than the UNIX time.");

            return (long)value.Subtract(unix).TotalMilliseconds;
        }
    }
}
