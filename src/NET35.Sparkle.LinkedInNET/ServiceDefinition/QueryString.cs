
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, XmlElement(ElementName = "QueryString")]
    public class QueryString
    {
        public int MyProperty { get; set; }
    }
}
