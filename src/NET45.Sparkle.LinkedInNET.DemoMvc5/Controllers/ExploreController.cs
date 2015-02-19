
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
    using Newtonsoft.Json;
    using Sparkle.LinkedInNET.DemoMvc5.ViewModels.Explore;

    public class ExploreController : Controller
    {
        private LinkedInApi api;
        private DataService data;
        private LinkedInApiConfiguration apiConfig;
        private JsonSerializerSettings shareSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        };

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

                {
                    var pictures = this.api.Profiles.GetOriginalProfilePicture(user)
                        ?? new PictureUrls() { PictureUrl = new List<string>() };

                    {
                        var more = this.api.Profiles.GetProfilePicture(user, 120, 120);
                        if (more != null && more.PictureUrl != null)
                        {
                            pictures.PictureUrl.AddRange(more.PictureUrl);
                        }
                    }

                    this.ViewBag.Pictures = this.ViewData["ProfilePictures"] = pictures;
                }
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
            var profile = this.api.Profiles.GetProfileById(user, id, acceptLanguages, fields);

            {
                var pictures = this.api.Profiles.GetOriginalProfilePicture(user, profile.Id)
                     ?? new PictureUrls() { PictureUrl = new List<string>() };
                var more = this.api.Profiles.GetProfilePicture(user, profile.Id, 120, 120);
                if (more != null && more.PictureUrl != null)
                {
                    pictures.PictureUrl.AddRange(more.PictureUrl);
                }

                this.ViewBag.Pictures = this.ViewData["ProfilePictures"] = pictures;
            }

            return this.View(profile);
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

        public ActionResult SearchCompany(string keywords, int start = 0, int count = 20, string facet = null)
        {
            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var fields = FieldSelector.For<CompanySearch>()
                .WithFacets()
                .WithCompaniesDescription().WithCompaniesId()
                .WithCompaniesLogoUrl().WithCompaniesName()
                .WithCompaniesSquareLogoUrl().WithCompaniesStatus()
                .WithCompaniesWebsiteUrl()
                .WithCompaniesUniversalName();

            CompanySearch result;
            if (!string.IsNullOrEmpty(facet))
                result = this.api.Companies.FacetSearch(user, start, count, keywords, facet, fields);
            else
                result = this.api.Companies.Search(user, start, count, keywords, fields);

            this.ViewBag.keywords = keywords;
            this.ViewBag.start = start;
            this.ViewBag.count = count;
            this.ViewBag.facet = facet;

            return this.View(result);
        }

        public ActionResult CompanyShare(string ShareId)
        {

            var item = new Sparkle.LinkedInNET.Common.PostShare
            {
                Visibility = new Visibility
                {
                    Code = "anyone",
                },
                Comment = "Testing a full company share with Sparkle.LinkedInNET in C#.NET!",
            };

            if (ShareId != null)
            {
                item.Attribution = new Attribution
                {
                    Share = ShareId,
                };
            }
            else
            {
                item.Content = new PostShareContent
                {
                    SubmittedUrl = "https://github.com/SparkleNetworks/LinkedInNET",
                    Title = "SparkleNetworks/LinkedInNET",
                    Description = "Sparkle.LinkedInNET will help you query the LinkedIn API with C# :)",
                    SubmittedImageUrl = "https://raw.githubusercontent.com/SparkleNetworks/LinkedInNET/master/src/LiNET-200.png",
                };
            }

            this.ViewBag.Share = item;

            var model = new CompanyShareModel
            {
                CompanyId = 2414183,
                Json = JsonConvert.SerializeObject(item, Formatting.Indented, shareSettings),
            };
            return this.View(model);
        }

        [HttpPost]
        public ActionResult CompanyShare(CompanyShareModel model)
        {
            var item = JsonConvert.DeserializeObject<Sparkle.LinkedInNET.Common.PostShare>(model.Json);
            this.ViewBag.Share = item;

            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var result = this.api.Companies.Share(user, model.CompanyId, item);

            this.ViewBag.Result = result != null ? result.Location : null;

            return this.View(model);
        }
    }
}
