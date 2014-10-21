
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// The result of a OAuth2/authorization.
    /// </summary>
    [DataContract]
    public class AuthorizationAccessToken
    {
        private DateTime authorizationDateUtc = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the validity of the token in seconds.
        /// </summary>
        [DataMember(IsRequired = false, Name = "expires_in")]
        public int? ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [DataMember(IsRequired = false, Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the authorization date UTC.
        /// </summary>
        [IgnoreDataMember]
        public DateTime AuthorizationDateUtc
        {
            get { return this.authorizationDateUtc; }
            set { this.authorizationDateUtc = value; }
        }
    }
}
