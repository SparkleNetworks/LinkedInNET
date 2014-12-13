
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class ImportFieldSelectors
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string ReturnType { get; set; }
    }
}
