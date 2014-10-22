
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Strongly-typed user access token for API calls.
    /// </summary>
    public class UserAuthorization
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthorization"/> class.
        /// </summary>
        public UserAuthorization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthorization"/> class.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        public UserAuthorization(string accessToken)
        {
            this.AccessToken = accessToken;
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
