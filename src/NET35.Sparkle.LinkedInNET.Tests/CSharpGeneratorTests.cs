
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
        public void ImplicitReturnTypes()
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
            Assert.IsTrue(result.Contains("public class R1"));
            Assert.IsTrue(result.Contains("public class R2"));
            Assert.IsTrue(result.Contains("public string F1"));
            Assert.IsTrue(result.Contains("public R2 F2"));
        }

        [TestMethod]
        public void MyTestMethod()
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
                        Methods = new List<ApiMethod>()
                        {
                            new ApiMethod
                            {
                                Path = "/v1/test1",
                                MethodName="mtd1",
                            },
                            new ApiMethod
                            {
                                Path = "/v1/test1/{UserId}",
                                MethodName="mtd2",
                            },
                        },
                        ReturnTypes = new List<ReturnType>(),
                    },
                },
            };
            generator.Run(root);

            writer.Flush();
            stream.Seek(0L, System.IO.SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();

            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("public void mtd1()"));
            Assert.IsTrue(result.Contains("public void mtd2(string UserId)"));
        }
    }
}
