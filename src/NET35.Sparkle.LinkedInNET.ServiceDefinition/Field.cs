
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class Field
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string PropertyName { get; set; }

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public bool IsDefault { get; set; }

        [XmlAttribute]
        public string ReturnType { get; set; }

        public string GetMainName()
        {
            var parts = this.Name.Split(new char[] { ':', }, 2);
            return parts[0];
        }

        public string GetSubName()
        {
            var parts = this.Name.Split(new char[] { ':', }, 2);
            return parts.Length == 2 ? parts[1] : null;
        }
    }
}
