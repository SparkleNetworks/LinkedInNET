
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    /// <summary>
    /// Exception for library internal errors.
    /// </summary>
    [Serializable]
    public class LinkedInNetException : Exception
    {
        internal LinkedInNetException()
        {
        }

        internal LinkedInNetException(string message)
            : base(message)
        {
        }

        internal LinkedInNetException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedInApiException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected LinkedInNetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
