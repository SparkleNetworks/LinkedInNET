
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;

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
            return this.FormatUrl(format, null, values);
        }

        protected string FormatUrl(string format, FieldSelector fieldSelector, params string[] values)
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

            if (fieldSelector != null)
            {
                result = result.Replace("{FieldSelector}", fieldSelector.ToString());
            }
            else
            {
                result = result.Replace("{FieldSelector}", string.Empty);
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

        internal bool ExecuteQuery(RequestContext context)
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

            if (context.AcceptLanguages != null)
            {
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, string.Join(",", context.AcceptLanguages));
            }

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

                return true;
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

                        var responseString = new StreamReader(context.ResponseStream, Encoding.UTF8).ReadToEnd();

                        context.ResponseStream.Seek(0L, SeekOrigin.Begin);
                        ////var error = this.HandleXmlResponse<ApiError>(context);
                        ////Exception ex1;
                        ////if (error != null)
                        ////{
                        ////    ex1 = FX.ApiException("ApiErrorResult", error.Status, error.Message);
                        ////}
                        ////else
                        ////{
                        ////    ex1 = FX.ApiException("ApiEmptyErrorResult", (int)(response.StatusCode));
                        ////}

                        ////ex1.Data["ApiRawResponse"] = responseString;
                        ////throw ex1;
                        return false;
                    }

                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + "): " + ex.Message, ex);
                }
                else
                {
                    throw new InvalidOperationException("Error from API: " + ex.Message, ex);
                }
            }
        }

        internal void HandleXmlErrorResponse(RequestContext context)
        {
            var error = this.HandleXmlResponse<ApiError>(context);

            Exception ex1;
            if (error != null)
            {
                ex1 = FX.ApiException("ApiErrorResult", error.Status, error.Message);
            }
            else
            {
                ex1 = FX.ApiException("ApiEmptyErrorResult", (int)(context.HttpStatusCode));
            }

            throw ex1;
        }

        private static int[] validHttpCodes = new int[] { 200, 201, 202, };
        private static int[] errorHttpCodes = new int[] { 400, 401, 403, 404, 500, };

        internal T HandleXmlResponse<T>(RequestContext context)
            where T : class, new()
        {
            T result = null;
            ApiError errorResult = null;
            ////try
            {
                var serializer = new XmlSerializer(typeof(T));
                var errorSerializer = new XmlSerializer(typeof(ApiError));

                if (validHttpCodes.Contains(context.HttpStatusCode))
                {
                    result = (T)serializer.Deserialize(context.ResponseStream);
                }
                else if (errorHttpCodes.Contains(context.HttpStatusCode))
                {
                    errorResult = (ApiError)serializer.Deserialize(context.ResponseStream);
                    var ex = FX.ApiException("ApiErrorResult", errorResult.Status, errorResult.Message);
                    ex.Data.Add("ErrorResult", errorResult);
                    throw ex;
                }
                else
                {
                    throw FX.ApiException("ApiUnknownError", context.HttpStatusCode);
                }
            }
            ////catch (LinkedInApiException)
            ////{
            ////    throw;
            ////}
            ////catch (Exception ex)
            ////{
            ////    throw new InvalidOperationException("Failed to read API response", ex);
            ////}

            if (result == null)
            {
                throw FX.ApiException("ApiEmptyResult");
            }

            return result;
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
