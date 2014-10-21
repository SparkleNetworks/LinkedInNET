
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UserAuthorization
    {
        public UserAuthorization()
        {
        }

        public UserAuthorization(string accessToken)
        {
            this.AccessToken = accessToken;
        }

        public string AccessToken { get; set; }
    }
}
