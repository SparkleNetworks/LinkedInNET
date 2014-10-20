
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable, XmlRoot]
    public class ApisRoot
    {
        [XmlElement(ElementName = "ApiGroup")]
        public IList<ApiGroup> ApiGroups { get; set; }
    }
}
