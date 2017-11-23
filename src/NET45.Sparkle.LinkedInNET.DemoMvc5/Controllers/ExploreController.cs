
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
    using System.Threading.Tasks;
    using System.Text;

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

        public async Task<ActionResult> Index(string culture = "en-US")
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
                    var pictures = await this.api.Profiles.GetOriginalProfilePictureAsync(user);
                    pictures = pictures ?? new PictureUrls() { PictureUrl = new List<string>(), };

                    {
                        var more = await this.api.Profiles.GetProfilePictureAsync(user, 120, 120);
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

        public async Task<ActionResult> Company(int id, string culture = "en-US", int start = 0, int count = 10, string eventType = null)
        {
            this.ViewBag.Id = id;
            this.ViewBag.EventType = eventType;

            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);
            var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };

            Exception error = null;
            Company company;
            {
                var fields = FieldSelector.For<Company>()
                    .WithAllFields();
                try
                {
                    company = await this.api.Companies.GetByIdAsync(user, id.ToString(), fields);
                }
                catch (LinkedInApiException ex)
                {
                    if (ex.StatusCode == 403)
                    {
                        company = new Company();
                        company.Id = id;
                        company.Name = "Company " + id + " - Permission error";
                        error = ex;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            this.ViewBag.SharesStart = start;
            this.ViewBag.SharesCount = count;
            this.ViewBag.SharesTotal = 0;
            if (error != null)
            {
                var fields = FieldSelector.For<CompanyUpdates>();
                Companies.CompanyUpdates shares;
                try
                {
                    if (string.IsNullOrEmpty(eventType))
                    {
                        shares = await this.api.Companies.GetSharesAsync(user, id, start, count);
                    }
                    else
                    {
                        shares = await this.api.Companies.GetSharesAsync(user, id, start, count, eventType);
                    }

                    this.ViewBag.SharesTotal = shares.Total;
                    this.ViewBag.Shares = shares;
                }
                catch (LinkedInApiException ex)
                {
                    if (ex.StatusCode == 403)
                    {
                        error = ex;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            this.ViewBag.Error = error;
            
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

        public ActionResult CompanyShare(string ShareId, string CompanyId)
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

            int id;
            if (!string.IsNullOrEmpty(CompanyId) && int.TryParse(CompanyId, out id))
            {
                model.CompanyId = id;
            }

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

            this.ViewBag.Result = result;

            return this.View(model);
        }

        public ActionResult UserShare(string ShareId)
        {
            // PERMISSION REQUIRED: rw_nus
            var item = new Sparkle.LinkedInNET.Common.PostShare
            {
                Visibility = new Visibility
                {
                    Code = "anyone",
                },
                Comment = "Testing a user share with Sparkle.LinkedInNET in C#.NET!",
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
                Json = JsonConvert.SerializeObject(item, Formatting.Indented, shareSettings),
            };
            return this.View(model);
        }

        [HttpPost]
        public ActionResult UserShare(CompanyShareModel model)
        {
            var item = JsonConvert.DeserializeObject<Sparkle.LinkedInNET.Common.PostShare>(model.Json);
            this.ViewBag.Share = item;

            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var result = this.api.Social.Post(user, item);

            this.ViewBag.Result = result;

            return this.View(model);
        }

        public ActionResult CompanyShareDetails(int CompanyId, string ShareId)
        {
            this.ViewBag.CompanyId = CompanyId;
            this.ViewBag.ShareId = ShareId;

            // demonstrates raw API queries
            // usefull when something is missing in the library
            // access this page using: /Explore/CompanyShareDetails?CompanyId=2414183&ShareId=UPDATE-c2414183-6339433061230342144
            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var jsonResponse = this.api.RawGetJsonQuery("/v1/companies/" + CompanyId + "/updates/key=" + Uri.EscapeDataString(ShareId) + "?format=json", user);

            // you may also use the low-level HTTP abstraction to fully customize the query
            /*
            var context = new Sparkle.LinkedInNET.Internals.RequestContext();
            context.Method = "POST";
            context.PostDataType = "application/json";
            context.RequestHeaders.Add("x-li-format", "json");
            context.PostData = Encoding.UTF8.GetBytes("{POST DATA HERE}");
            var result = this.api.RawQuery(context, "JSON");
            */

            this.ViewData.Model = jsonResponse;
            return this.View();
        }

        public async Task<ActionResult> CompaniesAdminList()
        {
            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var result = await this.api.Companies.GetAdminListAsync(user);
            return this.View(result);
        }
    }
}
