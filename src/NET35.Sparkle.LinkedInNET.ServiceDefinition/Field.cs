
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
        public bool IsCollection { get; set; }

        [XmlAttribute]
        public string ReturnType { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public bool Ignore { get; set; }

        [XmlElement(ElementName = "FieldSelector")]
        public List<FieldSelector> Selectors { get; set; }

        internal FieldName FieldName { get; set; }

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

        public FieldName[] GetNameParts()
        {
            return GetNameParts(this.Name, this.PropertyName);
        }

        public static FieldName[] GetNameParts(string name, string propertyName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The value cannot be empty", "name");

            string originalName = name;

            // strip enclosing paranthesis
            if (name[0] == '(' && name[name.Length - 1] == ')')
            {
                name = name.Substring(1, name.Length - 2);
                throw new ArgumentException("This method does not support subfields: '" + name + "'");
            }
            else
            {
                var parts = name.Split(new char[] { ':', '/', }, 2);
                if (parts.Length == 0)
                    return null;

                if (parts.Length == 1)
                {
                    return new FieldName[]
                {
                    new FieldName
                    {
                        ApiName = name,
                        PropertyName = propertyName ?? (propertyName = CSharpGenerator.Namify(name, NameTransformation.CamelCase)),
                        ClassName = null,
                    },
                };
                }

                var currentField = new FieldName
                {
                    ApiName = parts[0],
                    PropertyName = CSharpGenerator.Namify(parts[0], NameTransformation.CamelCase),
                };
                currentField.ClassName = currentField.PropertyName;

                // handle subfields or sub-property names
                FieldName[] subFields = null;
                if (IsFieldList(parts[1]))
                {
                    currentField.SubNames = GetFieldList(parts[1]);
                    if (currentField.SubNames.Length == 1)
                    {
                        subFields = GetNameParts(currentField.SubNames[0], null);
                    }
                }
                else
                {
                    subFields = GetNameParts(parts[1], null);
                }

                // create result array
                FieldName[] result;
                if (subFields != null && subFields.Length > 0)
                {
                    result = new FieldName[subFields.Length + 1];
                    Array.Copy(subFields, 0, result, 1, subFields.Length);
                }
                else
                {
                    result = new FieldName[1];
                }
                
                result[0] = currentField;

                return result;
            }
        }

        private static bool IsFieldList(string name)
        {
            return name[0] == '(' && name[name.Length - 1] == ')';
        }

        private static string[] GetFieldList(string name)
        {
            if (!IsFieldList(name))
                throw new ArgumentException("Argument must be a (fieldlist)");
            
            name = name.Substring(1, name.Length - 2);

            var parts = name.Split(new char[] { ':', });
            var result = new List<string>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (part[0] == '(' && part[part.Length - 1] == ')')
                {
                    result[result.Count - 1] += ":" + part;
                }
                else
                {
                    result.Add(part);
                }
            }

            return result.ToArray();
        }

        public override string ToString()
        {
            return string.Format("N='{0}' PN='{1}' T='{2}' Tl='{3}'", Name, PropertyName, Type, Title);
        }
    }
}
