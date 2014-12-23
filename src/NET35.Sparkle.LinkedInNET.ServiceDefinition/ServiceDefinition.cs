
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ServiceDefinition
    {
        private ApisRoot apisRoot;

        public ServiceDefinition(ApisRoot apisRoot)
        {
            this.apisRoot = apisRoot;
        }

        public ApisRoot Root
        {
            get { return this.apisRoot; }
        }

        internal ReturnType FindReturnType(string name, string apiGroupName = null, string subPart = null, string typeName = null)
        {
            // find return type or null
            ReturnType returnItem = null;

            // handle different namespaces
            int dotPosition = name.LastIndexOf('.');
            if (dotPosition > 0)
            {
                var ns = name.Substring(0, dotPosition);
                int dotPosition2 = ns.LastIndexOf('.');
                if (dotPosition2 > 0)
                {
                    ns = ns.Substring(0, dotPosition2);
                }

                if (apiGroupName != null)
                {
                    apiGroupName = ns;
                }

                name = name.Substring(dotPosition + 1);
            }

            foreach (var group in this.Root.ApiGroups.Where(g => apiGroupName == null || g.Name == apiGroupName))
            {
                if (returnItem != null)
                    break;

                foreach (var item in group.ReturnTypes)
                {
                    if (returnItem != null)
                        break;

                    if (item.Name == name || item.ClassName == name)
                    {
                        returnItem = item;
                    }
                }
            }

            // create return type in specified API group
            if (apiGroupName != null && returnItem == null)
            {
                var item = new ReturnType
                {
                    Name = name,
                    ClassName = typeName ?? CSharpGenerator.Namify(name, NameTransformation.CamelCase),
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
                    // subPart = "main:(sub)"
                    // subPart = "main:(sub:(woot))"
                    Field newField = null;
                    {
                        var parts = subPart.Substring(1, subPart.Length - 2).Split(new char[] { ':', }, 2);
                        newField = new Field
                        {
                            Name = parts[0],
                            ReturnType = name,
                            Type = parts.Length > 1 && parts[1].Length > 2 ? CSharpGenerator.Namify(parts[0]) : null,
                        };
                        returnItem.Fields.Add(newField);
                    }

                    {
                        // split "main:(sub:(woot))"
                        var parts = subPart.Substring(1, subPart.Length - 2).Split(new char[] { ':', '/', }, 2);
                        var mainPart = parts.Length == 1 ? parts[0] : parts[0];
                        var subPart1 = parts.Length == 2 ? parts[1] : null;

                        if (parts.Length > 1)
                        {
                            // "(sub:(woot))"
                            var subReturnType = this.FindReturnType(mainPart, apiGroupName, subPart: subPart1);
                        }
                    }
                }
                else if (subPart != null)
                {
                    // "main/sub"
                    var parts = subPart.Split(new char[] { ':', '/', }, 2);
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
