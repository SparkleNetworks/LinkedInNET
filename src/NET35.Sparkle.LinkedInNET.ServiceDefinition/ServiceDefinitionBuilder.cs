
namespace Sparkle.LinkedInNET.ServiceDefinition
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class ServiceDefinitionBuilder
    {
        private ApisRoot root;
        private ServiceDefinition definition;
        private bool isDirty = false;
        private List<string> warnings;
        private List<string> infos;

        public ApisRoot Root
        {
            get
            {
                if (this.isDirty)
                {
                    this.Sanitize();
                }

                return this.root;
            }
        }

        public ServiceDefinition Definition
        {
            get
            {
                if (this.isDirty)
                {
                    this.Sanitize();
                }

                return this.definition;
            }
        }

        public void AppendServiceDefinition(Stream stream)
        {
            var streamPosition = stream.Position;

            var serializer = new XmlSerializer(typeof(ApisRoot));
            var reader = new StreamReader(stream);
            var root = (ApisRoot)serializer.Deserialize(reader);

            if (stream.CanSeek)
                stream.Seek(streamPosition, SeekOrigin.Begin);

            this.AppendRoot(root);
        }

        public void AppendServiceDefinition(ServiceDefinition serviceDefinition)
        {
            this.AppendRoot(serviceDefinition.Root);
        }

        private void AppendRoot(ApisRoot tree)
        {
            this.isDirty = true;

            if (this.root == null)
            {
                this.root = tree;
            }
            else
            {
                foreach (var apiGroup in tree.ApiGroups)
                {
                    var matchGroup = this.root.ApiGroups.SingleOrDefault(g => g.Name == apiGroup.Name);
                    if (matchGroup != null)
                    {
                        matchGroup.Methods.AddRange(apiGroup.Methods);
                        matchGroup.ReturnTypes.AddRange(apiGroup.ReturnTypes);
                    }
                    else
                    {
                        this.root.ApiGroups.Add(apiGroup);
                    }
                }
            }
        }

        private void Sanitize()
        {
            this.isDirty = false;
            this.warnings = new List<string>();
            this.infos = new List<string>();

            this.definition = this.definition ?? (this.definition = new ServiceDefinition(this.root));

            this.VerifyFieldNames();

            {
                foreach (var apiGroup in this.root.ApiGroups)
                {
                    // generate implicit return types
                    foreach (var returnType in apiGroup.ReturnTypes.ToArray())
                    {
                        foreach (var item in returnType.Fields.ToArray())
                        {
                            var parts = item.Name.Split(new char[] { ':', '/', }, 2);
                            var mainPart = parts.Length == 1 ? parts[0] : parts[0];
                            var subPart = parts.Length == 2 ? parts[1] : null;

                            if (parts.Length > 1)
                            {
                                var subReturnType = definition.FindReturnType(mainPart, apiGroup.Name, subPart: subPart, typeName: item.Type);
                            }
                        }
                    }
                }
            }

            this.VerifyFieldNames();
        }

        private void VerifyFieldNames()
        {
            foreach (var apiGroup in this.root.ApiGroups)
            {
                // fill optional stuff
                foreach (var returnType in apiGroup.ReturnTypes.ToArray())
                {
                    returnType.ApiGroup = apiGroup.Name;

                    foreach (var item in returnType.Fields)
                    {
                        // field name stuff
                        if (string.IsNullOrEmpty(item.Name))
                        {
                            this.warnings.Add("Field " + apiGroup.Name + "/" + returnType.Name + "/" + returnType.Fields.IndexOf(item) + " has en empty name.");
                        }

                        var nameParts = item.GetNameParts();
                        item.FieldName = nameParts[0];
                        if (item.Type != null && item.FieldName.ClassName == null)
                            item.FieldName.ClassName = item.Type;

                        if (string.IsNullOrEmpty(item.PropertyName))
                        {
                            item.PropertyName = item.FieldName.PropertyName;
                            this.infos.Add("Set property name of " + apiGroup.Name + "/" + returnType.Name + "/" + returnType.Fields.IndexOf(item) + "-" + item.Name + " to " + item.PropertyName + ".");
                        }
                    }

                    if (returnType.Headers != null)
                    {
                        foreach (var item in returnType.Headers)
                        {
                            // field name stuff
                            if (string.IsNullOrEmpty(item.Name))
                            {
                                this.warnings.Add("Header " + apiGroup.Name + "/" + returnType.Name + "/" + returnType.Headers.IndexOf(item) + " has en empty name.");
                            }

                            var nameParts = item.GetNameParts();
                            item.FieldName = nameParts[0];
                            if (item.Type != null && item.FieldName.ClassName == null)
                                item.FieldName.ClassName = item.Type;

                            if (string.IsNullOrEmpty(item.PropertyName))
                            {
                                item.PropertyName = item.FieldName.PropertyName;
                                this.infos.Add("Set property name of " + apiGroup.Name + "/" + returnType.Name + "/" + returnType.Fields.IndexOf(item) + "-" + item.Name + " to " + item.PropertyName + ".");
                            }
                        }
                    }
                }
            }
        }
    }
}
