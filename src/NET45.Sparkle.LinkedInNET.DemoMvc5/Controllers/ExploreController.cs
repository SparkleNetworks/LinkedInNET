
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
                    .WithPositions().WithId().WithPictureUrl()
                    .WithFirstName().WithLastName().WithHeadline();
                    ////.WithConnections();
                this.ViewData["MyProfile"] = this.api.Profiles.GetMyProfile(user, acceptLanguages, fields);
                this.ViewBag.Profile = this.ViewData["MyProfile"];
                this.ViewData["ProfilePictures"] = this.api.Profiles.GetOriginalProfilePicture(user)
                    ?? new PictureUrls() { PictureUrl = new List<string>() };
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
                .WithId()
                .WithPositions().WithPictureUrl()
                .WithFirstName().WithLastName().WithHeadline()
                .WithLanguageId().WithLanguageName().WithLanguageProficiency()
                .WithConnections();
            var company = this.api.Profiles.GetProfileById(user, id, acceptLanguages, fields);
            //this.ViewData["ProfilePictures"] = this.api.Profiles.GetOriginalProfilePicture(user);
            //this.ViewBag.Pictures = this.ViewData["ProfilePictures"];

            return this.View(company);
        }

        public ActionResult Connections(string id, string culture = "en-US", int start = 0, int count = 10, int? days = null)
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);
            var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };

            {
                var fields = FieldSelector.For<Person>()
                    .WithFirstName().WithLastName().WithHeadline()
                    .WithPictureUrl();
                this.ViewBag.Profile = this.api.Profiles.GetMyProfile(user, acceptLanguages, fields);
            }

            Connections model;
            {
                var fields = FieldSelector.For<Connections>()
                    .WithDistance().WithPictureUrl()
                    .WithSummary().WithLocationName().WithLocationCountryCode()
                    .WithIndustry().WithId()
                    .WithPublicProfileUrl()
                    .WithFirstName().WithLastName().WithHeadline();

                if (days == null)
                {
                    model = this.api.Profiles.GetConnectionsByProfileId(user, id, start, count, fields);
                }
                else
                {
                    var date = DateTime.UtcNow.AddDays(-days.Value);
                    var modifiedSince = date.ToUnixTime();
                    model = this.api.Profiles.GetNewConnectionsByProfileId(user, id, start, count, modifiedSince, fields);
                }
            }

            this.ViewBag.id = id;
            this.ViewBag.start = start;
            this.ViewBag.days = days;
            this.ViewBag.count = count;
            this.ViewBag.days = days;

            return this.View(model);
        }
    }
}