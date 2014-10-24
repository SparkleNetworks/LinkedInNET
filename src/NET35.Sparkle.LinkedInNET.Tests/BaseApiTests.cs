
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

        [TestMethod]
        public void FormatUrl0WithFieldsButNoPlaceholder()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a", new FieldSelector<object>().Add("hello").Add("world"));
            Assert.AreEqual("a", result);
        }

        [TestMethod]
        public void FormatUrl0WithFieldsAndPlaceholder()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{FieldSelector}a", new FieldSelector<object>().Add("hello").Add("world"));
            Assert.AreEqual("a:(hello,world)a", result);
        }

        [TestMethod]
        public void FormatUrl1WithFieldsAndPlaceholder()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{b}a{FieldSelector}a", new FieldSelector<object>().Add("hello").Add("world"), "b", "c");
            Assert.AreEqual("aca:(hello,world)a", result);
        }

        [TestMethod]
        public void FormatUrlWithSlashFieldSelector()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{FieldSelector}a", FieldSelector.For<object>().Add("site-standard-profile-request/url"));
            Assert.AreEqual("a:(site-standard-profile-request)a", result);
        }
    }

    public class Api : BaseApi
    {
        public Api()
            : base(null)
        {
        }

        public Api(LinkedInApi linkedInApi)
            : base(linkedInApi)
        {
        }

        public string FormatUrlInvoke(string format, params string[] values)
        {
            return FormatUrl(format, values);
        }

        public string FormatUrlInvoke(string format, FieldSelector fields, params string[] values)
        {
            return FormatUrl(format, fields, values);
        }
    }
}
