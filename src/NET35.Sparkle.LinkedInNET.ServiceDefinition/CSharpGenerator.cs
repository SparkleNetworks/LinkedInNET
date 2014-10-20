
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class CSharpGenerator
    {
        private readonly TextWriter text;
        private string rootNamespace = "Sparkle.LinkedInNET";

        public CSharpGenerator(TextWriter text)
        {
            this.text = text;
        }

        public string RootNamespace
        {
            get { return this.rootNamespace; }
            set { this.rootNamespace = value; }
        }

        public void Run(ApisRoot root)
        {
            var context = new GeneratorContext();
            context.Root = root;

            this.WriteEverything(context);

            text.Flush();
        }

        private void WriteEverything(GeneratorContext context)
        {
            
            foreach (var apiGroup in context.Root.ApiGroups)
            {
                // generate extra return types
                foreach (var returnType in apiGroup.ReturnTypes.ToArray())
                {
                    foreach (var item in returnType.Fields)
                    {
                        var parts = item.Name.Split(new char[] { ':', }, 2);
                        var mainPart = parts.Length == 1 ? parts[0] : parts[0];
                        var subPart = parts.Length == 2 ? parts[1] : null;

                        if (parts.Length > 1)
                        {
                            var subReturnType = context.FindReturnType(mainPart, apiGroup.Name, subPart: subPart);
                        }
                    }
                }
            }

            foreach (var apiGroup in context.Root.ApiGroups)
            {
                // write all return types
                foreach (var returnType in apiGroup.ReturnTypes.ToArray())
                {
                    this.Write(context, returnType, apiGroup);
                }
            }

            /*

foreach (var apiGroup in root.ApiGroups)
{
#>
namespace <#=nameSpace#>.Services.<#=apiGroup.Name#>
{
	public class <#=apiGroup.Name#>Api
	{
<#
		foreach (var item in apiGroup.Methods) {
#>
		public <#=item.ReturnType#> <#=item.MethodName#>()
		{

		}
<#
		}
#>
	}


<#
	foreach (var returnType in apiGroup.ReturnTypes)
	{
#>
	public class <#=returnType.Name#>
	{

	}
<#
	}
#>

}
<#
}

            */

        }

        private void Write(GeneratorContext context, ReturnType returnType, ApiGroup apiGroup)
        {
            int indent = 0;
            this.text.WriteLine(indent, "namespace " + this.RootNamespace + "." + apiGroup.Name);
            this.text.WriteLine(indent++, "{");
            this.WriteNamespace(indent, "System");
            this.WriteNamespace(indent, "System.Xml.Serialization");
            this.text.WriteLine();
            this.text.WriteLine(indent, "[Serializable]");
            this.text.WriteLine(indent, "public class " + this.GetPropertyName(returnType.ClassName, returnType.Name));
            this.text.WriteLine(indent++, "{");

            foreach (var itemGroup in returnType.Fields.GroupBy(f => this.GetPropertyName(f.PropertyName, f.GetMainName())).ToArray())
            {
                ////int itemIndex = -1;
                ////foreach (var item in itemGroup)
                ////{
                ////    itemIndex++;
                ////}

                var item = itemGroup.First();
                var parts = item.Name.Split(new char[] { ':', }, 2);
                var mainPart = parts.Length == 1 ? parts[0] : parts[0];
                var subPart = parts.Length == 2 ? parts[1] : null;

                var type = "string";
                if (parts.Length > 1)
                {
                    var subReturnType = context.FindReturnType(mainPart, apiGroupName: apiGroup.Name, subPart: subPart);
                    if (subReturnType != null)
                    {
                        type = this.GetPropertyName(subReturnType.ClassName, subReturnType.Name);
                    }
                    else
                    {
                        type = this.GetPropertyName(null, mainPart);
                    }
                }

                this.text.WriteLine(indent, "/// <summary>");
                foreach (var subItem in itemGroup)
                {
                    this.text.WriteLine(indent, "/// Field: '" + subItem.Name + "'"); 
                }

                this.text.WriteLine(indent, "/// </summary>");
                this.text.WriteLine(indent, "[XmlElement(ElementName = \"" + mainPart + "\")]");
                this.text.WriteLine(indent, "public " + type + " " + this.GetPropertyName(item.PropertyName, mainPart) + " { get; set; }");
                this.text.WriteLine();
            }

            this.text.WriteLine(--indent, "}");
            this.text.WriteLine(--indent, "}");
            this.text.WriteLine();
        }

        private string GetPropertyName(string p1, string p2)
        {
            if (p1 != null)
                return p1;

            var words = p2.Split(new char[] { '-', '/', });

            return string.Join("", words.Select(w => w[0].ToString().ToUpperInvariant() + new string(w.Skip(1).ToArray())).ToArray());
        }

        private void WriteNamespace(int indent, string value)
        {
            this.text.WriteLine(indent, "using " + value + ";");
        }

        public class GeneratorContext
        {

            public ApisRoot Root { get; set; }

            internal ReturnType FindReturnType(string name, string apiGroupName = null, string subPart = null)
            {
                ReturnType returnItem = null;
                foreach (var group in this.Root.ApiGroups.Where(g => apiGroupName == null || g.Name == apiGroupName))
                {
                    if (returnItem != null)
                        break;

                    foreach (var item in group.ReturnTypes)
                    {
                        if (returnItem != null)
                            break;

                        if (item.Name == name)
                        {
                            returnItem = item;
                            return item;
                        }
                    }
                }

                if (apiGroupName != null)
                {
                    var item = new ReturnType
                    {
                        Name = name,
                        Fields = new List<Field>(),
                    };
                    var group = this.Root.ApiGroups.Single(g => g.Name == apiGroupName);
                    group.ReturnTypes.Add(item);

                    returnItem = item;
                }

                if (returnItem != null)
                {
                    if (subPart != null && subPart.First() == '(' && subPart.Last() == ')')
                    {
                        var parts = subPart.Substring(1, subPart.Length - 2).Split(new char[] { ':', }, 2);
                        returnItem.Fields.Add(new Field
                        {
                            Name = parts[0],
                            ReturnType = name,
                        });
                    }
                }

                return returnItem;
            }
        }
    }
}
