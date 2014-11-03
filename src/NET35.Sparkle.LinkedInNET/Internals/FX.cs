
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class FX
    {
        internal static LinkedInApiException ApiException(string name, params object[] args)
        {
            return ApiException(name, null, args);
        }

        internal static LinkedInApiException ApiException(string name, Exception innerException, params object[] args)
        {
            const string prefix = "ApiException_";
            LinkedInApiException ex;
            if (args == null || args.Length == 0)
            {
                var message = Strings.ResourceManager.GetString(prefix + name);
                ex = new LinkedInApiException(message, innerException);
            }
            else
            {
                var message = string.Format(Strings.ResourceManager.GetString(prefix + name), args);
                ex = new LinkedInApiException(message, innerException);
            }

            ex.Data["Key"] = name;

            return ex;
        }

        internal static LinkedInNetException InternalException(string name, params object[] args)
        {
            return InternalException(name, null, args);
        }

        internal static LinkedInNetException InternalException(string name, Exception innerException, params object[] args)
        {
            const string prefix = "InternalException_";
            LinkedInNetException ex;
            if (args == null || args.Length == 0)
            {
                var message = Strings.ResourceManager.GetString(prefix + name);
                ex = new LinkedInNetException(message, innerException);
            }
            else
            {
                var message = string.Format(Strings.ResourceManager.GetString(prefix + name), args);
                ex = new LinkedInNetException(message, innerException);
            }

            ex.Data["Key"] = name;

            return ex;
        }
    }
}
