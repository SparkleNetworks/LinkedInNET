
namespace Sparkle.LinkedInNET.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class LinkedInApiConfigurationTests
    {
        [TestClass]
        public class EmptyCtor
        {
            [TestMethod]
            public void Works()
            {
                var target = new LinkedInApiConfiguration();
                Assert.IsNull(target.ApiKey);
                Assert.IsNull(target.ApiSecretKey);
                Assert.IsNotNull(target.BaseApiUrl);
                Assert.IsNotNull(target.BaseOAuthUrl);
                Assert.AreEqual("https://api.linkedin.com", target.BaseApiUrl);
                Assert.AreEqual("https://www.linkedin.com", target.BaseOAuthUrl);
            }
        }

        [TestClass]
        public class ApiKeySecretCtor
        {
            [TestMethod]
            public void Works()
            {
                var target = new LinkedInApiConfiguration("hello", "world");
                Assert.IsNotNull(target.ApiKey);
                Assert.IsNotNull(target.ApiSecretKey);
                Assert.IsNotNull(target.BaseApiUrl);
                Assert.IsNotNull(target.BaseOAuthUrl);
                Assert.AreEqual("hello", target.ApiKey);
                Assert.AreEqual("world", target.ApiSecretKey);
                Assert.AreEqual("https://api.linkedin.com", target.BaseApiUrl);
                Assert.AreEqual("https://www.linkedin.com", target.BaseOAuthUrl);
            }
        }
    }
}
