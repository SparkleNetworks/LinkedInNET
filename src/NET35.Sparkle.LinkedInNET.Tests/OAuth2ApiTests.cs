
namespace Sparkle.LinkedInNET.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.LinkedInNET.OAuth2;

    [TestClass]
    public class OAuth2ApiTests
    {
        [TestClass]
        public class GetAuthorizationUrlMethod
        {
            [TestMethod]
            public void Works()
            {
                AuthorizationScope scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
                string state = "AZERTYUIOP";
                string redirectUri = "http://localhost/Callbacks/LinkedIN?Redirect=" + Uri.EscapeDataString("/Profile?LinkedIN");

                var config = new LinkedInApiConfiguration { ApiKey = "HELLOAPI", BaseOAuthUrl = "https://linkedin.com", };
                var api = new LinkedInApi(config);

                var result = api.OAuth2.GetAuthorizationUrl(scope, state, redirectUri);
                Assert.AreEqual("https://linkedin.com/uas/oauth2/authorization?response_type=code&client_id=HELLOAPI&scope=r_basicprofile%20r_emailaddress&state=AZERTYUIOP&redirect_uri=http%3A%2F%2Flocalhost%2FCallbacks%2FLinkedIN%3FRedirect%3D%252FProfile%253FLinkedIN", result.OriginalString);
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Arg1Empty()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAuthorizationUrl(AuthorizationScope.ReadEmailAddress, string.Empty, "hello");
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Arg2Empty()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAuthorizationUrl(AuthorizationScope.ReadEmailAddress, "hello", string.Empty);
            }

            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ChecksConfiguration()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAuthorizationUrl(AuthorizationScope.ReadEmailAddress, "hello", "world");
            }
        }

        [TestClass]
        public class GetAccessTokenMethod
        {
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Arg1Empty()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAccessToken(string.Empty, "hello");
            }

            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void Arg2Empty()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAccessToken("hello", string.Empty);
            }

            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ChecksConfiguration()
            {
                var api = new LinkedInApi(new LinkedInApiConfiguration());
                api.OAuth2.GetAccessToken("hello", "world");
            }
        }
    }
}
