
namespace Sparkle.LinkedInNET.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Sparkle.LinkedInNET.Internals;

    /// <summary>
    /// OAuth2 operations for LinkedIn.
    /// </summary>
    /// <remarks>
    /// https://developer.linkedin.com/documents/authentication
    /// </remarks>
    public class OAuth2Api : BaseApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2Api"/> class.
        /// </summary>
        /// <param name="linkedInApi">The linked information API.</param>
        public OAuth2Api(LinkedInApi linkedInApi)
            : base(linkedInApi)
        {
        }

        /// <summary>
        /// Gets the authorization URL.
        /// </summary>
        /// <param name="scope">An enum of permissions.</param>
        /// <param name="state">A unique identifier to use as a CSRF check.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// The value cannot be empty;state
        /// or
        /// The value cannot be empty;redirectUri
        /// </exception>
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

        /// <summary>
        /// Gets the access token for a authorization code.
        /// </summary>
        /// <param name="authorizationCode">The authorization code.</param>
        /// <param name="redirectUri">The redirect URI.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// The value cannot be empty;authorizationCode
        /// or
        /// The value cannot be empty;redirectUri
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Failed to read API response
        /// or
        /// Error from API (HTTP  + (int)(response.StatusCode) + )
        /// or
        /// Error from API:  + ex.Message
        /// or
        /// API responded with an empty response
        /// </exception>
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

            var context = new RequestContext
            {
                Method= "POST",
                UrlPath = url,
            };
            this.ExecuteQuery(context);

            AuthorizationAccessToken result = null;
            OAuth2Error errorResult = null;

            // read response content
            try
            {
                if (context.HttpStatusCode == 200 || context.HttpStatusCode == 201)
                {
#if NET35
                    var reader = new StreamReader(context.ResponseStream, Encoding.UTF8);
                    var json = reader.ReadToEnd();
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationAccessToken>(json);
#else
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(AuthorizationAccessToken));
                    result = (AuthorizationAccessToken)serializer.ReadObject(context.ResponseStream);
#endif

                    result.AuthorizationDateUtc = DateTime.UtcNow;
                }
                else
                {
#if NET35
                    var reader = new StreamReader(context.ResponseStream, Encoding.UTF8);
                    var json = reader.ReadToEnd();
                    errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuth2Error>(json);
#else
                    var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(OAuth2Error));
                    errorResult = (OAuth2Error)serializer.ReadObject(context.ResponseStream);
#endif
                    throw FX.ApiException("OAuth2ErrorResult", errorResult.Error, errorResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read API response", ex);
            }

            if (result == null)
            {
                throw new InvalidOperationException("API responded with an empty response");
            }

            return result;
        }
    }
}
