
namespace Sparkle.LinkedInNET.DemoMvc5.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Ninject;
    using Sparkle.LinkedInNET.DemoMvc5.Domain;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.LinkedInNET.Profiles;

    public class HomeController : Controller
    {
        private LinkedInApi api;
        private DataService data;
        private LinkedInApiConfiguration apiConfig;

        public HomeController(LinkedInApi api, DataService data, LinkedInApiConfiguration apiConfig)
        {
            this.api = api;
            this.data = data;
            this.apiConfig = apiConfig;
        }

        public ActionResult Index()
        {
            // step 1: configuration
            this.ViewBag.Configuration = this.apiConfig;
            
            // step 2: authorize url
            var scope = AuthorizationScope.ReadFullProfile | AuthorizationScope.ReadEmailAddress;
            var state = Guid.NewGuid().ToString();
            var redirectUrl = this.Request.Compose() + this.Url.Action("OAuth2");
            this.ViewBag.LocalRedirectUrl = redirectUrl;
            if (this.apiConfig != null && !string.IsNullOrEmpty(this.apiConfig.ApiKey))
            {
                var authorizeUrl = this.api.OAuth2.GetAuthorizationUrl(scope, state, redirectUrl);
                this.ViewBag.Url = authorizeUrl;
            }
            else
            {
                this.ViewBag.Url = null;
            }

            // step 3
            if (this.data.HasAccessToken)
            {
                var token = this.data.GetAccessToken();
                this.ViewBag.Token = token;
                var user = new UserAuthorization(token);

                var watch = new Stopwatch();
                watch.Start();
                try
                {
                    ////var profile = this.api.Profiles.GetMyProfile(user);
                    ////var full = this.api.Profiles.GetMyProfile();


                    ////var fields = FieldSelector.For<Person>().WithSummary().WithPictureUrl().WithPhoneticLastName().Add("").WithPersonalInfos();
                    ////this.api.Profiles.GetMyProfile(user, fields);
                    ////this.api.Profiles.GetMyProfile(user);
                    var profile = this.api.Profiles.GetMyProfile(user, FieldSelector.For<Person>().WithAllFields());

                    this.ViewBag.Profile = profile;
                }
                catch (Exception ex)
                {
                    this.ViewBag.ProfileError = ex.ToString();
                }
                watch.Stop();
                this.ViewBag.ProfileDuration = watch.Elapsed;
            }

            return this.View();
        }

        public ActionResult OAuth2(string code, string state)
        {
            var redirectUrl = this.Request.Compose() + this.Url.Action("OAuth2");
            var result = this.api.OAuth2.GetAccessToken(code, redirectUrl);

            this.ViewBag.Code = code;
            this.ViewBag.Token = result.AccessToken;

            this.data.SaveAccessToken(result.AccessToken);

            var user = new UserAuthorization(result.AccessToken);

            ////var profile = this.api.Profiles.GetMyProfile(user);
            ////this.data.SaveAccessToken();
            return this.View();
        }
    }
}
