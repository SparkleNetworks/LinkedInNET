
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.LinkedInNET.ServiceDefinition;

    [TestClass]
    public class CSharpGeneratorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var stream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(stream);
            var generator = new CSharpGenerator(writer);
            var root = new ApisRoot
            {
                ApiGroups = new List<ApiGroup>()
                {
                    new ApiGroup
                    {
                        Name = "g",
                        ReturnTypes = new List<ReturnType>()
                        {
                            new ReturnType
                            {
                                Name = "r1",
                                Fields = new List<Field>()
                                {
                                    new Field
                                    {
                                        Name = "f1",
                                    },
                                    new Field
                                    {
                                        Name = "f2:(r2:(name))",
                                    },
                                },
                            },
                        },
                    },
                },
            };
            generator.Run(root);

            writer.Flush();
            stream.Seek(0L, System.IO.SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
