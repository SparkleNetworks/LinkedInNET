
namespace Sparkle.LinkedInNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// An API error.
    /// </summary>
    [Serializable, XmlRoot(ElementName = "error")]
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the (HTTP?) status.
        /// </summary>
        [XmlElement(ElementName = "status")]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [XmlElement(ElementName = "timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the request unique identifier.
        /// </summary>
        [XmlElement(ElementName = "request-id")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [XmlElement(ElementName = "error-code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [XmlElement(ElementName = "message")]
        public string Message { get; set; }
    }
}
