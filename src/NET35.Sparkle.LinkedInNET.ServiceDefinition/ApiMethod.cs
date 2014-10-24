
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [Serializable]
    public class ApiMethod
    {
        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string ReturnType { get; set; }

        [XmlAttribute]
        public string MethodName { get; set; }

        [XmlAttribute]
        public string HttpMethod { get; set; }

        [XmlAttribute]
        public bool RequiresUserAuthentication { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", HttpMethod, Path, MethodName, Title);
        }
    }
}
