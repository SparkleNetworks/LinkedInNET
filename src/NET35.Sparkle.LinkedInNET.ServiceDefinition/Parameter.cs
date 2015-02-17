
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Parameter
    {
        public Parameter()
        {
        }

        public Parameter(string name, string type = null, string value = null, string originalName = null)
        {
            this.Name = name;
            this.Value = value;
            this.OriginalName = originalName ?? name;
            this.Type = type;
        }

        public string OriginalName { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
