
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="Root")]
    public class ApisRoot
    {
        [XmlElement(ElementName = "ApiGroup")]
        public List<ApiGroup> ApiGroups { get; set; }

        [XmlElement(ElementName = "ReturnType")]
        public List<ReturnType> ReturnTypes { get; set; }
    }
}
