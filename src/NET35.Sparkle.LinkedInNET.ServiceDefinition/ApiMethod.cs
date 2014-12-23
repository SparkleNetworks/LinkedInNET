
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
        private bool useFieldSelectors = true;

        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string ReturnType { get; set; }

        [XmlAttribute]
        public string PostReturnType { get; set; }

        [XmlAttribute]
        public string MethodName { get; set; }

        [XmlAttribute]
        public string HttpMethod { get; set; }

        [XmlAttribute]
        public bool RequiresUserAuthentication { get; set; }

        [XmlAttribute]
        public bool UsesAcceptLanguage { get; set; }

        [XmlAttribute]
        public bool UseFieldSelectors
        {
            get { return this.useFieldSelectors; }
            set { this.useFieldSelectors = value; }
        }

        ////[XmlAttribute]
        ////public List<Field> QueryFields { get; set; }

        public override string ToString()
        {
            return string.Format("ApiMethod({0}{1}{2}{3})", HttpMethod, Path, MethodName, Title);
        }
    }
}
