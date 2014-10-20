
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.LinkedInNET.Internals;

    [TestClass]
    public class BaseApiTests
    {
        [TestMethod]
        public void FormatUrl0()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a");
            Assert.AreEqual("a", result);
        }

        [TestMethod]
        public void FormatUrl1()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{b}a", "b", "c");
            Assert.AreEqual("aca", result);
        }

        [TestMethod]
        public void FormatUrl2()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{b}a{d}a", "b", "c", "d", "e");
            Assert.AreEqual("acaea", result);
        }
    }

    public class Api : BaseApi
    {
        public string FormatUrlInvoke(string format, params string[] values)
        {
            return FormatUrl(format, values);
        }
    }
}
