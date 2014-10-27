
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class ReturnType
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ClassName { get; set; }

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Remark { get; set; }

        ////[XmlAttribute]
        ////public bool IsDefault { get; set; }

        [XmlElement(ElementName = "Field")]
        public List<Field> Fields { get; set; }

        [XmlElement(ElementName = "IsCollectionOf")]
        public string IsCollectionOf { get; set; }

        public override string ToString()
        {
            return string.Format("N='{0}' CN='{1}' F={2}", Name, ClassName, Fields != null ? Fields.Count : 0);
        }
    }
}
