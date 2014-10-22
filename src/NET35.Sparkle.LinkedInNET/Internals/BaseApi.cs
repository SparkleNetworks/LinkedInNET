
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Base class for LinkedIn APIs.
    /// </summary>
    public class BaseApi
    {
        private LinkedInApi linkedInApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApi"/> class.
        /// </summary>
        /// <param name="linkedInApi">The API factory.</param>
        internal protected BaseApi(LinkedInApi linkedInApi)
        {
            this.linkedInApi = linkedInApi;
        }

        internal LinkedInApi LinkedInApi
        {
            get { return this.linkedInApi; }
        }

        ////internal I Http
        ////{
        ////    get {  }
        ////}

        /// <summary>
        /// Formats the URL.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        protected string FormatUrl(string format, params string[] values)
        {
            var result = format;

            var dic = new Dictionary<string, string>(values.Length / 2);
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                {

                }
                else
                {
                    dic.Add(values[i - 1], values[i]);
                }
            }

            foreach (var key in dic.Keys)
            {
                result = result.Replace("{" + key + "}", dic[key]);
            }

            return result;
        }

        internal void CheckConfiguration(bool apiKey = false, bool apiSecretKey = false)
        {
            var config = this.linkedInApi.Configuration;
            if (config == null)
                throw new InvalidOperationException("Configuration is not set");

            if (apiSecretKey)
                apiKey = true;

            if (apiKey && string.IsNullOrEmpty(config.ApiKey))
                throw new InvalidOperationException("Missing API Key in configuration");

            if (apiSecretKey && string.IsNullOrEmpty(config.ApiSecretKey))
                throw new InvalidOperationException("Missing API Secret Key in configuration");
        }

        internal void ExecuteQuery(RequestContext context)
        {
            // https://developer.linkedin.com/documents/request-and-response-headers

            if (context == null)
                throw new ArgumentNullException("context");
            if (string.IsNullOrEmpty(context.Method))
                throw new ArgumentException("The value cannot be empty", "context.Method");
            if (string.IsNullOrEmpty(context.UrlPath))
                throw new ArgumentException("The value cannot be empty", "context.UrlPath");

            var request = (HttpWebRequest)HttpWebRequest.Create(context.UrlPath);
            request.Method = context.Method;
            request.UserAgent = LibraryInfo.UserAgent;

            // post stuff?

            // user authorization
            if (context.UserAuthorization != null)
            {
                if (string.IsNullOrEmpty(context.UserAuthorization.AccessToken))
                    throw new ArgumentException("The value cannot be empty", "context.UserAuthorization.AccessToken");

                request.Headers.Add("Authorization", "Bearer " + context.UserAuthorization.AccessToken);
            }

            // get response
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                context.HttpStatusCode = (int)response.StatusCode;

                var readStream = response.GetResponseStream();
                BufferizeResponse(context, readStream);

                // check HTTP code
                if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created))
                {
                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + ")");
                }
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;

                if (response != null)
                {
                    context.HttpStatusCode = (int)response.StatusCode;

                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        BufferizeResponse(context, stream);
                    }

                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + "): " + ex.Message, ex);
                }
                else
                {
                    throw new InvalidOperationException("Error from API: " + ex.Message, ex);
                }
            }
        }

        private static void BufferizeResponse(RequestContext context, Stream readStream)
        {
            if (context.BufferizeResponseStream)
            {
                var memory = new MemoryStream();
                context.ResponseStream = memory;
                byte[] buffer = new byte[1024 * 1024];
                int readBytes = 0;
                while ((readBytes = readStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memory.Write(buffer, 0, readBytes);
                }

                memory.Seek(0L, SeekOrigin.Begin);
            }
            else
            {
                context.ResponseStream = readStream;
            }
        }
    }
}
