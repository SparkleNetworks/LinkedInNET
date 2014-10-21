
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
                        Methods = new List<ApiMethod>(),
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

        [TestClass]
        public class GetUrlPathParametersMethod
        {
            [TestMethod]
            public void BasicVariable()
            {
                var path = "hello{World}?nice";
                var target = new TestCSharpGenerator();
                var items = target.InvokeGetUrlPathParameters(path);

                Assert.AreEqual(1, items.Count);
                Assert.IsTrue(items.ContainsKey("World"));
                Assert.AreEqual("World", items["World"].OriginalName);
                Assert.AreEqual("World", items["World"].Name);
                Assert.IsNull(items["World"].Type);
            }

            [TestMethod]
            public void TypedVariable()
            {
                var path = "hello{DateTime World}?nice";
                var target = new TestCSharpGenerator();
                var items = target.InvokeGetUrlPathParameters(path);

                Assert.AreEqual(1, items.Count);
                Assert.IsTrue(items.ContainsKey("World"));
                Assert.AreEqual("World", items["World"].OriginalName);
                Assert.AreEqual("World", items["World"].Name);
                Assert.AreEqual("DateTime", items["World"].Type);
            }

            [TestMethod]
            public void TypedAndSimpleVariable()
            {
                var path = "hello{DateTime World}?nice{test}";
                var target = new TestCSharpGenerator();
                var items = target.InvokeGetUrlPathParameters(path);

                Assert.AreEqual(2, items.Count);
                Assert.IsTrue(items.ContainsKey("World"));
                Assert.AreEqual("World", items["World"].OriginalName);
                Assert.AreEqual("World", items["World"].Name);
                Assert.AreEqual("DateTime", items["World"].Type);
                Assert.IsTrue(items.ContainsKey("test"));
                Assert.AreEqual("test", items["test"].OriginalName);
                Assert.AreEqual("test", items["test"].Name);
                Assert.IsNull(items["test"].Type);
            }
        }

        public class TestCSharpGenerator : CSharpGenerator
        {
            public TestCSharpGenerator()
                : base(null)
            {
            }

            internal IDictionary<string, Parameter> InvokeGetUrlPathParameters(string path, NameTransformation transform = NameTransformation.None)
            {
                return base.GetUrlPathParameters(path, transform);
            }
        }
    }
}
