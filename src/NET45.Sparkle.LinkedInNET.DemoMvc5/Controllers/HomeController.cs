
namespace Sparkle.LinkedInNET.DemoMvc5.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Ninject;
    using Sparkle.LinkedInNET.DemoMvc5.Domain;
    using Sparkle.LinkedInNET.OAuth2;

    public class HomeController : Controller
    {
        private LinkedInApi api;
        private DataService data;

        public HomeController(LinkedInApi api, DataService data)
        {
            this.api = api;
            this.data = data;
        }

        public ActionResult Index()
        {
            var scope = AuthorizationScope.ReadBasicProfile | AuthorizationScope.ReadEmailAddress;
            var state = Guid.NewGuid().ToString();
            var redirectUrl = this.Request.Compose() + this.Url.Action("OAuth2");
            var authorizeUrl = this.api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);

            this.ViewBag.Url = authorizeUrl;

            return this.View();
        }

        public ActionResult OAuth2(string code, string state)
        {
            var redirectUrl = this.Request.Compose() + this.Url.Action("OAuth2");
            var result = this.api.OAuth2.GetAccessToken(code, redirectUrl);

            this.ViewBag.Code = code;
            this.ViewBag.Token = result.AccessToken;

            var user = new UserAuthorization(result.AccessToken);

            ////var profile = this.api.Profiles.GetMyProfile(user);
            ////this.data.SaveAccessToken();
            return this.View();
        }
    }
}