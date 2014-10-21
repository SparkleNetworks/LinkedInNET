
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.LinkedInNET.OAuth2;

    public partial class LinkedInApi
    {
        private readonly LinkedInApiConfiguration configuration;

        public LinkedInApi(LinkedInApiConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.configuration = configuration.Clone();
        }

        public OAuth2Api OAuth2
        {
            get { return new OAuth2Api(this); }
        }

        internal LinkedInApiConfiguration Configuration
        {
            get { return this.configuration; }
        }
    }
}
