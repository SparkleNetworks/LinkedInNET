
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
            Assert.AreEqual("a:(site-standard-profile-request/url)a", result);
        }

        [TestMethod]
        public void DefaultValueAndType()
        {
            var target = new Api();
            var result = target.FormatUrlInvoke("a{bool b = false}z", "bool b = false", true.ToString());
            Assert.AreEqual("aTruez", result);
        }

        [TestMethod]
        public void NullableDateTimeGetsConvertedToUnixTs()
        {
            var target = new Api();
            DateTime? date = new DateTime(2015, 4, 4, 16, 32, 17, 3, DateTimeKind.Utc);
            var result = target.FormatUrlInvoke("a{DateTime? date}z", "DateTime? date", date);
            Assert.AreEqual("a1428165137003z", result);
        }

        [TestMethod]
        public void DateTimeGetsConvertedToUnixTs()
        {
            var target = new Api();
            DateTime date = new DateTime(2015, 4, 4, 16, 32, 17, 3, DateTimeKind.Utc);
            var result = target.FormatUrlInvoke("a{DateTime date}z", "DateTime date", date);
            Assert.AreEqual("a1428165137003z", result);
        }

        ////[TestMethod]
        ////public void FormatUrlWithQueryString()
        ////{
        ////    var target = new Api();
        ////    var result = target.FormatUrlInvoke("path?a=1&{Query}");
        ////    Assert.AreEqual("aca", result);
        ////}
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

        public string FormatUrlInvoke(string format, params object[] values)
        {
            return FormatUrl(format, values);
        }

        public string FormatUrlInvoke(string format, FieldSelector fields, params object[] values)
        {
            return FormatUrl(format, fields, values);
        }
    }
}
