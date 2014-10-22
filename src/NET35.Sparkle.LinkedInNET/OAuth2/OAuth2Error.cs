
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// OAuth2 error.
    /// </summary>
    [DataContract]
    public class OAuth2Error
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        [DataMember(Name = "error")]
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember(Name = "error_description")]
        public string ErrorMessage { get; set; }
    }
}
