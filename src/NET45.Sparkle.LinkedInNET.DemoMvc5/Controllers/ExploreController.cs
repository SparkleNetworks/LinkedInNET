
namespace Sparkle.LinkedInNET.DemoMvc5.Controllers
{
    using Sparkle.LinkedInNET.DemoMvc5.Domain;
    using Sparkle.LinkedInNET.Companies;
    using Sparkle.LinkedInNET.Profiles;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Sparkle.LinkedInNET.Common;

    public class ExploreController : Controller
    {
        private LinkedInApi api;
        private DataService data;
        private LinkedInApiConfiguration apiConfig;

        public ExploreController(LinkedInApi api, DataService data, LinkedInApiConfiguration apiConfig)
        {
            this.api = api;
            this.data = data;
            this.apiConfig = apiConfig;
        }

        public ActionResult Index(string culture = "en-US")
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);
            var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };

            {
                var fields = FieldSelector.For<Person>()
                    .WithPositions()
                    .WithFirstName().WithLastName().WithHeadline()
                    .WithConnections();
                this.ViewData["MyProfile"] = this.api.Profiles.GetMyProfile(user, acceptLanguages, fields);
                this.ViewBag.Profile = this.ViewData["MyProfile"];
                this.ViewData["ProfilePictures"] = this.api.Profiles.GetOriginalProfilePicture(user);
                this.ViewBag.Pictures = this.ViewData["ProfilePictures"];
            }

            ////{
            ////    var companies = this.api.Companies.GetList(user);
            ////    this.ViewBag.Companies = companies;
            ////}

            return this.View();
        }

        public ActionResult Company(int id, string culture = "en-US")
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);
            var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };

            Company company;
            {
                var fields = FieldSelector.For<Company>()
                    .WithAllFields();
                company = this.api.Companies.GetById(user, id.ToString(), fields);
            }

            {
                var fields = FieldSelector.For<CompanyUpdates>();
                var shares = this.api.Companies.GetShares(user, id);
                this.ViewBag.Shares = shares;
            }

            return this.View(company);
        }

        public ActionResult Person(string id, string culture = "en-US")
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);
            var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };

            var fields = FieldSelector.For<Person>()
                .WithPositions()
                .WithFirstName().WithLastName().WithHeadline()
                .WithConnections();
            var company = this.api.Profiles.GetProfileById(user, id, acceptLanguages, fields);
            this.ViewData["ProfilePictures"] = this.api.Profiles.GetOriginalProfilePicture(user);
            this.ViewBag.Pictures = this.ViewData["ProfilePictures"];

            return this.View(company);
        }
    }
}