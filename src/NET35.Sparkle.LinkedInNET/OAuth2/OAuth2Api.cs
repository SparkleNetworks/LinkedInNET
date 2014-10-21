
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Sparkle.LinkedInNET.Internals;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// https://developer.linkedin.com/documents/authentication
    /// </remarks>
    public class OAuth2Api : BaseApi
    {
        public OAuth2Api(LinkedInApi linkedInApi)
            : base(linkedInApi)
        {
        }

        public Uri GetAuthorizationUrl(AuthorizationScope scope, string state, string redirectUri)
        {
            if (string.IsNullOrEmpty(state))
                throw new ArgumentException("The value cannot be empty", "state");
            if (string.IsNullOrEmpty(redirectUri))
                throw new ArgumentException("The value cannot be empty", "redirectUri");

            this.CheckConfiguration(apiKey: true);

            var flags = Enum.GetValues(typeof(AuthorizationScope))
                .Cast<AuthorizationScope>()
                .Where(s => (scope & s) == s)
                .Select(s => s.GetAuthorizationName())
                .ToArray();
            var scopeAsString = string.Join(" ", flags);

            var url = string.Format(
                "{0}/uas/oauth2/authorization?response_type=code&client_id={1}&scope={2}&state={3}&redirect_uri={4}",
                this.LinkedInApi.Configuration.BaseOAuthUrl,
                Uri.EscapeDataString(this.LinkedInApi.Configuration.ApiKey),
                Uri.EscapeDataString(scopeAsString),
                Uri.EscapeDataString(state),
                Uri.EscapeDataString(redirectUri));
            return new Uri(url);
        }

        public AuthorizationAccessToken GetAccessToken(string authorizationCode, string redirectUri)
        {
            if (string.IsNullOrEmpty(authorizationCode))
                throw new ArgumentException("The value cannot be empty", "authorizationCode");
            if (string.IsNullOrEmpty(redirectUri))
                throw new ArgumentException("The value cannot be empty", "redirectUri");

            this.CheckConfiguration(apiSecretKey: true);

            var url = string.Format(
                "{0}/uas/oauth2/accessToken?grant_type=authorization_code&code={1}&redirect_uri={2}&client_id={3}&client_secret={4}",
                this.LinkedInApi.Configuration.BaseOAuthUrl,
                Uri.EscapeDataString(authorizationCode),
                Uri.EscapeDataString(redirectUri),
                Uri.EscapeDataString(this.LinkedInApi.Configuration.ApiKey),
                Uri.EscapeDataString(this.LinkedInApi.Configuration.ApiSecretKey));

            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "Sparkle.LinkedInNET";
            
            // get response
            HttpWebResponse response;
            string json;
            AuthorizationAccessToken result = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var readStream = response.GetResponseStream();
                var reader = new StreamReader(readStream, Encoding.UTF8);
                json = reader.ReadToEnd();

                // read response content
                try
                {
                    result = JsonConvert.DeserializeObject<AuthorizationAccessToken>(json);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to read API response", ex);
                }

                // check HTTP code
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + ")");
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;

                throw new InvalidOperationException("Error from API: " + ex.Message, ex);
            }

            if (result == null)
            {
                throw new InvalidOperationException("API responded with an empty response");
            }

            return result;
        }
    }
}
