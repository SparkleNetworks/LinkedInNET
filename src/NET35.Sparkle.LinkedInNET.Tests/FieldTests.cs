
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.LinkedInNET.ServiceDefinition;

    [TestClass]
    public class FieldTests
    {
        [TestClass]
        public class GetNamePartsMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void EmptyNameThrows()
            {
                var target = new Field();
                var parts = target.GetNameParts();
            }

            [TestMethod]
            public void SimpleName()
            {
                var target = new Field
                {
                    Name = "name",
                };
                var parts = target.GetNameParts();
                Assert.AreEqual(1, parts.Length);
                Assert.AreEqual("name", parts[0].ApiName);
                Assert.AreEqual("Name", parts[0].PropertyName);
                Assert.IsNull(parts[0].ClassName);
            }

            [TestMethod]
            public void SimpleNameAndPropertyName()
            {
                var target = new Field
                {
                    Name = "name",
                    PropertyName = "MyName",
                };
                var parts = target.GetNameParts();
                Assert.AreEqual(1, parts.Length);
                Assert.AreEqual("name", parts[0].ApiName);
                Assert.AreEqual("MyName", parts[0].PropertyName);
                Assert.IsNull(parts[0].ClassName);
            }

            [TestMethod]
            public void TwoNamesParenthesis()
            {
                var name = "name:(code)";
                var parts = Field.GetNameParts(name, null);
                Assert.AreEqual(2, parts.Length);
                Assert.AreEqual("name", parts[0].ApiName);
                Assert.AreEqual("Name", parts[0].PropertyName);
                Assert.AreEqual("Name", parts[0].ClassName);
                Assert.AreEqual("code", parts[1].ApiName);
                Assert.AreEqual("Code", parts[1].PropertyName);
                Assert.IsNull(parts[1].ClassName);
            }

            [TestMethod]
            public void ThreeNamesParenthesis()
            {
                var name = "name:(code:(sub-code))";
                var parts = Field.GetNameParts(name, null);
                Assert.AreEqual(3, parts.Length);
                Assert.AreEqual("name", parts[0].ApiName);
                Assert.AreEqual("Name", parts[0].PropertyName);
                Assert.AreEqual("Name", parts[0].ClassName);
                Assert.AreEqual("code", parts[1].ApiName);
                Assert.AreEqual("Code", parts[1].PropertyName);
                Assert.AreEqual("Code", parts[1].ClassName);
                Assert.AreEqual("sub-code", parts[2].ApiName);
                Assert.AreEqual("SubCode", parts[2].PropertyName);
                Assert.IsNull(parts[2].ClassName);
            }
        }
    }
}
