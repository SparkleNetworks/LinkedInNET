
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    [DataContract]
    public class AuthorizationAccessToken
    {
        private DateTime authorizationDateUtc = DateTime.UtcNow;
        
        [DataMember(IsRequired = false, Name = "expires_in")]
        public int? ExpiresIn { get; set; }

        [DataMember(IsRequired = false, Name = "access_token")]
        public string AccessToken { get; set; }

        [IgnoreDataMember]
        public DateTime AuthorizationDateUtc
        {
            get { return this.authorizationDateUtc; }
            set { this.authorizationDateUtc = value; }
        }
    }
}
