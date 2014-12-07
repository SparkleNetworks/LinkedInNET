
namespace Sparkle.LinkedInNET.DemoMvc5.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Ninject;
    using Sparkle.LinkedInNET.DemoMvc5.Domain;
    using Sparkle.LinkedInNET.OAuth2;
    using Sparkle.LinkedInNET.Profiles;
    using Sparkle.LinkedInNET.Companies;
    ////using Sparkle.LinkedInNET.ServiceDefinition;

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

        public ActionResult Index(string culture = "en-US")
        {
            // step 1: configuration
            this.ViewBag.Configuration = this.apiConfig;
            
            // step 2: authorize url
            var scope = AuthorizationScope.ReadFullProfile | AuthorizationScope.ReadEmailAddress | AuthorizationScope.ReadNetwork | AuthorizationScope.ReadContactInfo;
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
                    var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };
                    var fields = FieldSelector.For<Person>()
                        .WithId()
                        .WithFirstName()
                        .WithLastName()
                        .WithFormattedName()
                        .WithEmailAddress()
                        .WithHeadline()

                        .WithLocationName()
                        .WithLocationCountryCode()

                        .WithPictureUrl()
                        .WithPublicProfileUrl()
                        .WithSummary()
                        .WithIndustry()

                        .WithPositions()
                        .WithThreeCurrentPositions()
                        .WithThreePastPositions()

                        .WithProposalComments()
                        .WithAssociations()
                        .WithInterests()
                        .WithLanguageId()
                        .WithLanguageName()
                        .WithLanguageProficiency()
                        .WithCertifications()
                        .WithEducations()
                        .WithFullVolunteer()
                        .WithPatents()
                        ////.WithRecommendationsReceived() // may not use that
                        .WithDateOfBirth()
                        .WithPhoneNumbers()
                        .WithImAccounts()
                        .WithPrimaryTwitterAccount()
                        .WithTwitterAccounts()
                        .WithSkills();
                    var profile = this.api.Profiles.GetMyProfile(user, acceptLanguages, fields);

                    var originalPicture = this.api.Profiles.GetOriginalProfilePicture(user);
                    this.ViewBag.Picture = originalPicture;

                    this.ViewBag.Profile = profile;
                }
                catch (LinkedInApiException ex)
                {
                    this.ViewBag.ProfileError = ex.ToString();
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

        public ActionResult Connections()
        {
            var token = this.data.GetAccessToken();
            var user = new UserAuthorization(token);
            var connection = this.api.Profiles.GetMyConnections(user, 0, 500);
            return this.View(connection);
        }

        public ActionResult FullProfile(string id, string culture = "en-US")
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            var user = new UserAuthorization(token);

            Person profile = null;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                ////var profile = this.api.Profiles.GetMyProfile(user);
                var acceptLanguages = new string[] { culture ?? "en-US", "fr-FR", };
                var fields = FieldSelector.For<Person>()
                    .WithFirstName().WithFormattedName().WithLastName()
                    .WithHeadline()
                    .WithId()
                    .WithEmailAddress()

                    //.WithLocation()
                    .WithLocationName()        // subfields issue
                    //.WithLocationCountryCode() // subfields issue

                    .WithPictureUrl()
                    .WithPublicProfileUrl()
                    .WithSummary()
                    .WithIndustry()

                    .WithPositions()
                    .WithThreeCurrentPositions()
                    .WithThreePastPositions()

                    .WithProposalComments()
                    .WithAssociations()
                    .WithInterests()
                    .WithLanguageId()
                    .WithLanguageName()
                    .WithLanguageProficiency()
                    .WithCertifications()
                    .WithEducations()
                    .WithFullVolunteer()
                    //.WithRecommendationsReceived() // may not use that
                    .WithDateOfBirth()
                    .WithPhoneNumbers()
                    .WithImAccounts()
                    .WithPrimaryTwitterAccount()
                    .WithTwitterAccounts()
                    .WithAllFields()
                    ;
                profile = this.api.Profiles.GetProfileById(user, id, acceptLanguages, fields);

                this.ViewBag.Profile = profile;
            }
            catch (LinkedInApiException ex)
            {
                this.ViewBag.ProfileError = ex.ToString();
                this.ViewBag.RawResponse = ex.Data["ResponseText"];
            }
            catch (LinkedInNetException ex)
            {
                this.ViewBag.ProfileError = ex.ToString();
                this.ViewBag.RawResponse = ex.Data["ResponseText"];
            }
            catch (Exception ex)
            {
                this.ViewBag.ProfileError = ex.ToString();
            }

            watch.Stop();
            this.ViewBag.ProfileDuration = watch.Elapsed;

            return this.View(profile);
        }

        public ActionResult Play()
        {
            var token = this.data.GetAccessToken();
            this.ViewBag.Token = token;
            return this.View();
        }

        public ActionResult Definition()
        {
            var filePath = Path.Combine(this.Server.MapPath("~"), "..", "LinkedInApi.xml");
            var builder = new Sparkle.LinkedInNET.ServiceDefinition.ServiceDefinitionBuilder();
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                builder.AppendServiceDefinition(fileStream);
            }

            var result = new ApiResponse<Sparkle.LinkedInNET.ServiceDefinition.ApisRoot>(builder.Root);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public class ApiResponse<T>
        {
            public ApiResponse()
            {
            }

            public ApiResponse(T data)
            {
                this.Data = data;
            }

            public string Error { get; set; }
            public T Data { get; set; }
        }
    }
}
