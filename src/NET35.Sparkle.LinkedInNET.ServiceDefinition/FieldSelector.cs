
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class FieldSelector
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string PropertyName { get; set; }

        [XmlAttribute]
        public string Title { get; set; }
    }
}
