
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Exception for API errors.
    /// </summary>
    [Serializable]
    public class LinkedInApiException : Exception
    {
        internal LinkedInApiException()
        {
        }

        internal LinkedInApiException(string message)
            : base(message)
        {
        }

        internal LinkedInApiException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedInApiException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LinkedInApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
