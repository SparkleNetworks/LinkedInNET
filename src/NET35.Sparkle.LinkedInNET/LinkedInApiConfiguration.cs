
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LinkedInApiConfiguration
    {
        public LinkedInApiConfiguration()
        {
            this.BaseApiUrl = "https://api.linkedin.com";
            this.BaseOAuthUrl = "https://www.linkedin.com";
        }

        public string BaseApiUrl { get; set; }
        
        public string BaseOAuthUrl { get; set; }

        public string ApiKey { get; set; }

        public string ApiSecretKey { get; set; }

        internal LinkedInApiConfiguration Clone()
        {
            return new LinkedInApiConfiguration
            {
                BaseApiUrl = this.BaseApiUrl,
                BaseOAuthUrl = this.BaseOAuthUrl,
                ApiKey = this.ApiKey,
                ApiSecretKey = this.ApiSecretKey,
            };
        }
    }
}
