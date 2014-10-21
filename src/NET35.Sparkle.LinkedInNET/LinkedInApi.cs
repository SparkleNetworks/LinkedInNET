
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sparkle.LinkedInNET.OAuth2;

    /// <summary>
    /// LinkedIn API client.
    /// </summary>
    public partial class LinkedInApi
    {
        private readonly LinkedInApiConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedInApi"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public LinkedInApi(LinkedInApiConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.configuration = configuration.Clone();
        }

        /// <summary>
        /// Gets the OAuth2 API.
        /// </summary>
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
