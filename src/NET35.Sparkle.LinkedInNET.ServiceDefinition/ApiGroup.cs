
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class ApiGroup
    {
        [XmlAttribute(AttributeName="Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "ApiMethod")]
        public List<ApiMethod> Methods { get; set; }

        [XmlElement(ElementName = "ReturnType")]
        public List<ReturnType> ReturnTypes { get; set; }

        public override string ToString()
        {
            return "ApiGroup(N='" + Name + "', M=" + (Methods != null ? Methods.Count : 0) + ", RT=" + (ReturnTypes != null ? ReturnTypes.Count : 0) + ")";
        }
    }
}
