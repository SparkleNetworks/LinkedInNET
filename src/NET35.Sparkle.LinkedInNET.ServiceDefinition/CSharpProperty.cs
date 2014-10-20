
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CSharpProperty
    {
        public CSharpProperty(string type, string name)
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return this.ToString(0);
        }

        private string ToString(int indent)
        {
            var b = new StringBuilder();
            b.Append("public ");
            b.Append(this.Type);
            b.Append(" ");
            b.Append(this.Name);
            b.Append(" { get; set; }");

            return b.ToString();
        }
    }
}
