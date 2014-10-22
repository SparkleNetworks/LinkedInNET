
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class CSharpGenerator
    {
        private readonly TextWriter text;
        private string rootNamespace = "Sparkle.LinkedInNET";
        private static Regex urlParametersRegex = new Regex("\\{(?:([^{} ]+) +)?([^{}]+)\\}", RegexOptions.Compiled);

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
            }

            {
                foreach (var apiGroup in context.Root.ApiGroups)
                {
                    // write all return types
                    foreach (var returnType in apiGroup.ReturnTypes.ToArray())
                    {
                        this.WriteReturnTypes(context, returnType, apiGroup);
                    }
                }

                foreach (var apiGroup in context.Root.ApiGroups)
                {
                    // write client class
                    this.WriteApiGroup(context, apiGroup);
                }

                this.WriteRootServices(context);
            }


        }

        private void WriteRootServices(GeneratorContext context)
        {
            int indent = 0;
            this.text.WriteLine(indent, "namespace " + this.RootNamespace);
            this.text.WriteLine(indent++, "{");
            this.WriteNamespace(indent, "System");
            this.WriteNamespace(indent, "System.Xml.Serialization");
            this.WriteNamespace(indent, this.RootNamespace + ".Internals");

            foreach (var item in context.Root.ApiGroups)
            {
                var name = this.GetPropertyName(null, item.Name);
                this.WriteNamespace(indent, this.RootNamespace + "." + name);
            }

            this.text.WriteLine();
            this.text.WriteLine(indent, "/// <summary>");
            this.text.WriteLine(indent, "/// The factory for LinkedIn APIs.");
            this.text.WriteLine(indent, "/// </summary>");
            this.text.WriteLine(indent, "public partial class LinkedInApi : BaseApi");
            this.text.WriteLine(indent++, "{");

            // ctor


            // methods
            foreach (var item in context.Root.ApiGroups)
            {
                var name = this.GetPropertyName(null, item.Name);
                this.text.WriteLine(indent, "/// <summary>");
                this.text.WriteLine(indent, "/// The " + name + " API.");
                this.text.WriteLine(indent, "/// </summary>");
                this.text.WriteLine(indent++, "public " + name + "Api " + name + "{");
                this.text.WriteLine(indent, "get { return new "+name+"Api(this); }");
                this.text.WriteLine(--indent, "}");
                this.text.WriteLine();
            }

            this.text.WriteLine(--indent, "}");
            this.text.WriteLine(--indent, "}");
            this.text.WriteLine();

        }

        private void WriteApiGroup(GeneratorContext context, ApiGroup apiGroup)
        {
            var className = this.GetPropertyName(null, apiGroup.Name) + "Api";

            int indent = 0;
            this.text.WriteLine(indent, "namespace " + this.RootNamespace + "." + apiGroup.Name);
            this.text.WriteLine(indent++, "{");
            this.WriteNamespace(indent, "System");
            this.WriteNamespace(indent, "System.Xml.Serialization");
            this.WriteNamespace(indent, this.RootNamespace + ".Internals");
            this.text.WriteLine();
            this.text.WriteLine(indent, "/// <summary>");
            this.text.WriteLine(indent, "/// Name: '" + apiGroup.Name + "'");
            this.text.WriteLine(indent, "/// </summary>");
            this.text.WriteLine(indent, "public class " + className + " : BaseApi");
            this.text.WriteLine(indent++, "{");

            // ctor

            this.text.WriteLine(indent, "internal " + className + "(LinkedInApi linkedInApi)");
            this.text.WriteLine(indent, "    : base(linkedInApi)");
            this.text.WriteLine(indent++, "{");
            this.text.WriteLine(--indent, "}");
            this.text.WriteLine(indent, "");


            // methods
            WriteMethod(context, apiGroup, indent);

            this.text.WriteLine(--indent, "}");
            this.text.WriteLine(--indent, "}");
            this.text.WriteLine();
        }

        private void WriteMethod(GeneratorContext context, ApiGroup apiGroup, int indent)
        {
            foreach (var method in apiGroup.Methods)
            {
                var returnType = "void";
                ReturnType returnTypeType = null;
                if (method.ReturnType != null)
                {
                    returnTypeType = context.FindReturnType(method.ReturnType, apiGroup.Name);
                    if (returnTypeType != null)
                    {
                        returnType = this.GetPropertyName(returnTypeType.ClassName, returnTypeType.Name);
                    }
                }


                var parameters = new List<TupleStruct<string, string>>();

                if (method.RequiresUserAuthentication)
                {
                    parameters.Add(new TupleStruct<string, string>("UserAuthorization", "user"));
                }

                var urlParams = this.GetUrlPathParameters(method.Path, NameTransformation.PascalCase);
                foreach (var urlParam in urlParams)
                {
                    parameters.Add(new TupleStruct<string, string>(urlParam.Value.Type ?? "string", urlParam.Value.Name));
                }

                // doc
                this.text.WriteLine(indent, "/// <summary>");
                this.text.WriteLine(indent, "/// " + method.Title);
                this.text.WriteLine(indent, "/// </summary>");

                // name and arguments
                this.text.WriteLine(indent++, "public " + returnType + " " + this.GetPropertyName(method.MethodName, method.Path) + "(");

                var sep = "  ";
                foreach (var parameter in parameters)
                {
                    this.text.WriteLine(indent, sep + parameter.Value1 + " " + parameter.Value2);
                    sep = ", ";
                }

                this.text.WriteLine(--indent, ")");
                this.text.WriteLine(indent++, "{");

                // body / format url
                if (urlParams.Count > 0)
                {
                    this.text.WriteLine(indent, "const string urlFormat = \"" + method.Path + "\";");
                    this.text.WriteLine(indent, "var url = FormatUrl(urlFormat, " + string.Join(", ", urlParams.Values.Select(p => "\"" + p.OriginalName + "\", " + p.Name).ToArray()) + ");");
                }
                else
                {
                    this.text.WriteLine(indent, "var url = \"" + method.Path + "\";");
                }

                // body / create context
                this.text.WriteLine();
                text.WriteLine(indent, "var context = new RequestContext();");

                if (method.RequiresUserAuthentication)
                {
                    text.WriteLine(indent, "context.UserAuthorization = user;");
                }

                text.WriteLine(indent, "context.Method =  \"" + method.HttpMethod + "\";");
                text.WriteLine(indent, "context.UrlPath = this.LinkedInApi.Configuration.BaseApiUrl + url;");

                // body / execute
                this.text.WriteLine();
                text.WriteLine(indent, "this.ExecuteQuery(context);");

                // body / handle


                // body / return
                this.text.WriteLine();
                if (false /*method.ReturnRawResult*/)
                {
                    text.WriteLine(indent, "return response;");
                }
                else
                {
                    text.WriteLine(indent, "");
                    if (returnTypeType != null)
                    {
                        text.WriteLine(indent, "////var response = new System.IO.StreamReader(context.ResponseStream).ReadToEnd();");
                        text.WriteLine(indent, "var serializer = new XmlSerializer(typeof(" + returnType + "));");
                        text.WriteLine(indent, "var result = (" + returnType + ")serializer.Deserialize(context.ResponseStream);");
                    }
                    else
                    {
                        text.WriteLine(indent, "var result = JsonConvert.DeserializeObject<BaseResponse>(response);");
                    }

                    ////text.WriteLine(indent, "this.HandleErrors(result);");

                    if (returnType != null)
                        text.WriteLine(indent, "return result;");
                }

                ////this.text.WriteLine(indent, "throw new NotImplementedException(url);");
                this.text.WriteLine(--indent, "}");
                this.text.WriteLine(indent, "");
            }
        }

        protected IDictionary<string, Parameter> GetUrlPathParameters(string path, NameTransformation transform = NameTransformation.None)
        {
            var values = new Dictionary<string, Parameter>();
            var matches = urlParametersRegex.Matches(path);
            foreach (Match match in matches)
            {
                var key = match.Groups[2].Captures[0].Value;
                var type = match.Groups[1].Success ? match.Groups[1].Captures[0].Value : null;
                var item = new Parameter
                {
                    OriginalName = key,
                    Name = Namify(key, transform),
                    Type = type,
                };

                values.Add(key, item);
            }

            return values;
        }

        private void WriteReturnTypes(GeneratorContext context, ReturnType returnType, ApiGroup apiGroup)
        {
            int indent = 0;
            this.text.WriteLine(indent, "namespace " + this.RootNamespace + "." + apiGroup.Name);
            this.text.WriteLine(indent++, "{");
            this.WriteNamespace(indent, "System");
            this.WriteNamespace(indent, "System.Collections.Generic");
            this.WriteNamespace(indent, "System.Xml.Serialization");
            this.text.WriteLine();
            this.text.WriteLine(indent, "/// <summary>");
            this.text.WriteLine(indent, "/// Name: '" + returnType.Name + "'");
            this.text.WriteLine(indent, "/// </summary>");
            if (returnType.Remark != null)
            {
                this.text.WriteLine(indent, "/// <remarks>");
                this.text.WriteLine(indent, "/// " + returnType.Remark + "");
                this.text.WriteLine(indent, "/// </remarks>");
            }
            this.text.WriteLine(indent, "[Serializable, XmlRoot(\""+returnType.Name+"\")]");
            this.text.WriteLine(indent, "public class " + this.GetPropertyName(returnType.ClassName, returnType.Name));
            this.text.WriteLine(indent++, "{");

            foreach (var itemGroup in returnType.Fields.GroupBy(f => this.GetPropertyName(f.PropertyName, f.GetMainName())).ToArray())
            {
                var item = itemGroup.First();
                var parts = item.Name.Split(new char[] { ':', }, 2);
                var mainPart = parts.Length == 1 ? parts[0] : parts[0];
                var subPart = parts.Length == 2 ? parts[1] : null;
                var isCollection = item.IsCollection;

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

                if (isCollection)
                {
                    type = "List<" + type + ">";
                }

                this.text.WriteLine(indent, "/// <summary>");
                foreach (var subItem in itemGroup)
                {
                    this.text.WriteLine(indent, "/// Field: '" + subItem.Name + "' (" + (subItem.IsDefault ? "default" : "on-demand") + ")");
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

        private string GetPropertyName(string propertyName, string name)
        {
            if (propertyName != null)
                return propertyName;

            var words = name.Split(new char[] { '-', '/', });

            return string.Join("", words.Select(w => Namify(w, NameTransformation.CamelCase)).ToArray());
        }

        private static string Namify(string value, NameTransformation transform = NameTransformation.CamelCase, string unifier = "")
        {
            var result = string.Empty;
            var words = value.Split(new char[] { '-', '/', });
            var newWords = new string[words.Length];

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if ((transform & NameTransformation.PascalCase) == NameTransformation.PascalCase)
                {
                    word = word[0].ToString().ToLowerInvariant() + new string(word.Skip(1).ToArray());
                }

                if ((transform & NameTransformation.CamelCase) == NameTransformation.CamelCase)
                {
                    word = word[0].ToString().ToUpperInvariant() + new string(word.Skip(1).ToArray());
                }

                newWords[i] = word;
            }


            return string.Join(unifier, newWords);
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

        [Flags]
        public enum NameTransformation
        {
            None = 0,
            CamelCase  = 0x0001,
            PascalCase = 0x0002,
        }
    }
}
