
#if ASYNCTASKS
namespace Sparkle.LinkedInNET.Internals
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for LinkedIn APIs.
    /// </summary>
    partial class BaseApi
    {
        internal async Task<bool> ExecuteQueryAsync(RequestContext context)
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
            request.Headers.Add("x-li-format", "json");

            if (context.AcceptLanguages != null)
            {
                request.Headers.Add(HttpRequestHeader.AcceptLanguage, string.Join(",", context.AcceptLanguages));
            }

            // user authorization
            if (context.UserAuthorization != null)
            {
                if (string.IsNullOrEmpty(context.UserAuthorization.AccessToken))
                    throw new ArgumentException("The value cannot be empty", "context.UserAuthorization.AccessToken");

                request.Headers.Add("Authorization", "Bearer " + context.UserAuthorization.AccessToken);
            }

            foreach (var header in context.RequestHeaders)
            {
                request.Headers[header.Key] = header.Value;
            }

            // post stuff?
            if (context.PostData != null)
            {
                try
                {
                    if (context.PostDataType != null)
                        request.ContentType = context.PostDataType;

                    ////request.ContentLength = context.PostData.Length;
                    var stream = await request.GetRequestStreamAsync();
                    await stream.WriteAsync(context.PostData, 0, context.PostData.Length);
                    await stream.FlushAsync();
                }
                catch (WebException ex)
                {
                    throw new InvalidOperationException("Error POSTing to API (" + ex.Message + ")", ex);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error POSTing to API (" + ex.Message + ")", ex);
                }
            }

            // get response
            HttpWebResponse response;
            WebException webException = null;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
                context.HttpStatusCode = (int)response.StatusCode;
                context.ResponseHeaders = response.Headers;

                var readStream = response.GetResponseStream();
                await BufferizeResponseAsync(context, readStream);

                // check HTTP code
                if (!(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created))
                {
                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + ")");
                }

                return true;
            }
            catch (WebException ex)
            {
                webException = ex;
            }

            if (webException != null)
            {
                response = (HttpWebResponse)webException.Response;

                if (response != null)
                {
                    context.HttpStatusCode = (int)response.StatusCode;
                    context.ResponseHeaders = response.Headers;

                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        await BufferizeResponseAsync(context, stream);

                        var responseString = await new StreamReader(context.ResponseStream, Encoding.UTF8).ReadToEndAsync();

                        context.ResponseStream.Seek(0L, SeekOrigin.Begin);
                        return false;
                    }

                    throw new InvalidOperationException("Error from API (HTTP " + (int)(response.StatusCode) + "): " + webException.Message, webException);
                }
                else
                {
                    throw new InvalidOperationException("Error from API: " + webException.Message, webException);
                }
            }
            else
            {
                return true;
            }
        }

        private static async Task BufferizeResponseAsync(RequestContext context, Stream readStream)
        {
            if (context.BufferizeResponseStream)
            {
                var memory = new MemoryStream();
                context.ResponseStream = memory;
                byte[] buffer = new byte[1024 * 1024];
                int readBytes = 0;
                while ((readBytes = await readStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await memory.WriteAsync(buffer, 0, readBytes);
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
#endif
