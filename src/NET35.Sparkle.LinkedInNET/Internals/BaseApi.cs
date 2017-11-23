
namespace Sparkle.LinkedInNET.Internals
{
    using Newtonsoft.Json;
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
    public partial class BaseApi
    {
        private static int[] validHttpCodes = new int[] { 200, 201, 202, };
        private static int[] errorHttpCodes = new int[] { 400, 401, 403, 404, 500, };

        private LinkedInApi linkedInApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApi"/> class.
        /// </summary>
        /// <param name="linkedInApi">The API factory.</param>
        [System.Diagnostics.DebuggerStepThrough]
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
        protected string FormatUrl(string format, params object[] values)
        {
            return this.FormatUrl(format, null, values);
        }

        /// <summary>
        /// Formats the URL.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="fieldSelector">The field selectors.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        protected string FormatUrl(string format, FieldSelector fieldSelector, params object[] values)
        {
            var result = format;

            var dic = new Dictionary<string, string>(values.Length / 2);
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 1)
                {
                    var key = values[i - 1].ToString();
                    object valueObject = values[i] != null ? values[i] : null;
                    var value = valueObject != null ? valueObject.ToString() : null;

                    if (valueObject != null)
                    {
                        var type = valueObject.GetType();
                        DateTime? ndt = null;

                        if (type == typeof(DateTime?))
                        {
                            ndt = (DateTime?)valueObject;
                        }

                        if (type == typeof(DateTime))
                        {
                            ndt = (DateTime)valueObject;
                        }

                        if (ndt != null)
                        {
                            value = ndt.Value.ToUnixTime().ToString();
                        }
                    }

                    dic.Add(key, value);
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
                var value = dic[key];
                if (value != null)
                {
                    result = result.Replace("{" + key + "}", value);
                }
                else
                {
                    result = result.Replace("{" + key + "}", string.Empty);
                }
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
                    var stream = request.GetRequestStream();
                    stream.Write(context.PostData, 0, context.PostData.Length);
                    stream.Flush();
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
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                context.HttpStatusCode = (int)response.StatusCode;
                context.ResponseHeaders = response.Headers;

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
                    context.ResponseHeaders = response.Headers;

                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        BufferizeResponse(context, stream);

                        var responseString = new StreamReader(context.ResponseStream, Encoding.UTF8).ReadToEnd();

                        context.ResponseStream.Seek(0L, SeekOrigin.Begin);
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

        internal void HandleJsonErrorResponse(RequestContext context)
        {
            var error = this.HandleJsonResponse<ApiError>(context);

            LinkedInApiException ex1;
            if (error != null)
            {
                ex1 = FX.ApiException("ApiErrorResult", error.Status, error.Message);
                ex1.StatusCode = error.Status;
            }
            else
            {
                ex1 = FX.ApiException("ApiEmptyErrorResult", (int)(context.HttpStatusCode));
                ex1.StatusCode = (int)(context.HttpStatusCode);
            }

            throw ex1;
        }

        internal T1 ReadHeader<T1>(RequestContext context, string headerName)
        {
            if (context.ResponseHeaders == null)
                return default(T1);

            var value = context.ResponseHeaders[headerName];
            if (value == null)
                return default(T1);

            return (T1)Convert.ChangeType(value, typeof(T1));
        }

        internal void CreateJsonPostStream(RequestContext context, object postData)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };
            var json = JsonConvert.SerializeObject(postData, settings);
            var bytes = Encoding.UTF8.GetBytes(json);

            context.RequestHeaders.Add("x-li-format", "json");
            context.PostDataType = "application/json";
            context.PostData = bytes;
        }

        internal void CreateXmlPostStream(RequestContext context, object postData)
        {
            var ser = new XmlSerializer(postData.GetType());

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                ser.Serialize(stream, postData);
                stream.Seek(0L, SeekOrigin.Begin);
                bytes = stream.ToArray();
            }

            context.PostDataType = "application/xml";
            context.PostData = bytes;
        }

        internal T HandleXmlResponse<T>(RequestContext context)
            where T : class, new()
        {
            T result = null;
            ApiError errorResult = null;

            // create serializers
            // it may fail if attributes are wrong
            XmlSerializer serializer, errorSerializer;
            try
            {
                serializer = new XmlSerializer(typeof(T));
                errorSerializer = new XmlSerializer(typeof(ApiError));
            }
            catch (Exception ex)
            {
                throw FX.InternalException("SerializerCtor", ex, ex.Message);
            }

            if (validHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a success response
                try
                {
                    result = (T)serializer.Deserialize(context.ResponseStream);
                }
                catch (Exception ex)
                {
                    var ex1 = FX.InternalException("SerializerDeserialize", ex, ex.Message);
                    TryAttachContextDetails(context, ex1);
                    throw ex1;
                }
            }
            else if (errorHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a error response
                try
                {
                    errorResult = (ApiError)serializer.Deserialize(context.ResponseStream);
                }
                catch (Exception ex)
                {
                    var ex1 = FX.InternalException("SerializerDeserializeError", ex, ex.Message);
                    TryAttachContextDetails(context, ex1);
                    throw ex1;
                }

                {
                    var ex1 = FX.ApiException("ApiErrorResult", errorResult.Status, errorResult.Message, context.UrlPath);
                    TryAttachContextDetails(context, null);
                    ex1.Data.Add("ErrorResult", errorResult);
                    if (errorResult != null)
                    {
                        if (errorResult.Status == 401)
                            ex1.Data["ShouldRenewToken"] = true;
                    }

                    throw ex1;
                }
            }
            else
            {
                // unknown HTTP code
                var ex1 = FX.ApiException("ApiUnknownHttpCode", context.HttpStatusCode);
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            if (result == null)
            {
                var ex1 = FX.ApiException("ApiEmptyResult");
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            return result;
        }

        internal T HandleJsonResponse<T>(RequestContext context)
            where T : class, new()
        {
            T result = null;
            ApiError errorResult = null;

            string json;
            try
            {
                var reader = new StreamReader(context.ResponseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                var ex1 = FX.InternalException("ReadStreamAsText", ex, ex.Message);
                ex1.StatusCode = context.HttpStatusCode;
                throw ex1;
            }

            if (validHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a success response
                try
                {
                    result = JsonConvert.DeserializeObject<T>(json);
                }
                catch (Exception ex)
                {
                    var ex1 = FX.InternalException("SerializerDeserialize", ex, ex.Message);
                    TryAttachContextDetails(context, ex1);
                    throw ex1;
                }
            }
            else if (errorHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a error response
                ThrowJsonErrorResult(context, errorResult, json);
            }
            else
            {
                // unknown HTTP code
                var ex1 = FX.ApiException("ApiUnknownHttpCode", context.HttpStatusCode);
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            if (result == null)
            {
                var ex1 = FX.ApiException("ApiEmptyResult");
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            return result;
        }

        internal string HandleXmlRawResponse(RequestContext context)
        {
            ApiError errorResult = null;

            string xmlText;
            try
            {
                var reader = new StreamReader(context.ResponseStream, Encoding.UTF8);
                xmlText = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw FX.InternalException("ReadStreamAsText", ex, ex.Message);
            }

            if (validHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a success response
                // do nothing here
            }
            else if (errorHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a error response
                ThrowXmlErrorResult(context, errorResult, xmlText);
            }
            else
            {
                // unknown HTTP code
                var ex1 = FX.ApiException("ApiUnknownHttpCode", context.HttpStatusCode);
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            return xmlText;
        }

        internal string HandleJsonRawResponse(RequestContext context)
        {
            ApiError errorResult = null;

            string json;
            try
            {
                var reader = new StreamReader(context.ResponseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw FX.InternalException("ReadStreamAsText", ex, ex.Message);
            }

            if (validHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a success response
                // do nothing here
            }
            else if (errorHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a error response
                ThrowJsonErrorResult(context, errorResult, json);
            }
            else
            {
                // unknown HTTP code
                var ex1 = FX.ApiException("ApiUnknownHttpCode", context.HttpStatusCode);
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            return json;
        }

        internal string HandleRawResponse(RequestContext context, Encoding encoding)
        {
            string text;
            try
            {
                var reader = new StreamReader(context.ResponseStream, encoding);
                text = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw FX.InternalException("ReadStreamAsText", ex, ex.Message);
            }

            if (validHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a success response
                // do nothing here
            }
            else if (errorHttpCodes.Contains(context.HttpStatusCode))
            {
                // the HTTP code matches a error response
                var ex1 = FX.ApiException("ApiRawErrorResult");
                TryAttachContextDetails(context, ex1);
                throw ex1;
            }
            else
            {
                // unknown HTTP code
                var ex1 = FX.ApiException("ApiUnknownHttpCode", context.HttpStatusCode);
                TryAttachContextDetails(context, null);
                throw ex1;
            }

            return text;
        }

        private static void ThrowJsonErrorResult(RequestContext context, ApiError errorResult, string json)
        {
            try
            {
                errorResult = JsonConvert.DeserializeObject<ApiError>(json);
            }
            catch (Exception ex)
            {
                var ex1 = FX.InternalException("SerializerDeserializeError", ex, ex.Message);
                TryAttachContextDetails(context, ex1);
                throw ex1;
            }

            {
                var ex1 = FX.ApiException("ApiErrorResult", errorResult.Status, errorResult.Message, context.UrlPath);
                TryAttachContextDetails(context, ex1);
                ex1.Data.Add("ErrorResult", errorResult);
                if (errorResult != null)
                {
                    if (errorResult.Status == 401)
                        ex1.Data["ShouldRenewToken"] = true;
                }

                throw ex1;
            }
        }

        private static void ThrowXmlErrorResult(RequestContext context, ApiError errorResult, string json)
        {
            // create serializers
            // it may fail if attributes are wrong
            XmlSerializer errorSerializer;
            try
            {
                errorSerializer = new XmlSerializer(typeof(ApiError));
            }
            catch (Exception ex)
            {
                throw FX.InternalException("SerializerCtor", ex, ex.Message);
            }

            ApiError result;
            try
            {
                result = (ApiError)errorSerializer.Deserialize(context.ResponseStream);
            }
            catch (Exception ex)
            {
                var ex1 = FX.InternalException("SerializerDeserialize", ex, ex.Message);
                TryAttachContextDetails(context, ex1);
                throw ex1;
            }

            {
                var ex1 = FX.ApiException("ApiErrorResult", errorResult.Status, errorResult.Message, context.UrlPath);
                TryAttachContextDetails(context, ex1);
                ex1.Data.Add("ErrorResult", errorResult);
                if (errorResult != null)
                {
                    if (errorResult.Status == 401)
                        ex1.Data["ShouldRenewToken"] = true;
                }

                throw ex1;
            }
        }

        private static void TryAttachContextDetails(RequestContext context, ILinkedInException ex1)
        {
            if (context == null || ex1 == null)
                return;

            ex1.StatusCode = context.HttpStatusCode;

            try
            {
                var data = ((Exception)ex1).Data;
                data["ResponseStream"] = context.ResponseStream;
                data["AcceptLanguages"] = context.AcceptLanguages;
                data["BufferizeResponseStream"] = context.BufferizeResponseStream;
                data["HttpStatusCode"] = context.HttpStatusCode;
                data["Method"] = context.Method;
                data["UrlPath"] = context.UrlPath;

                if (context.BufferizeResponseStream)
                {
                    context.ResponseStream.Seek(0L, SeekOrigin.Begin);
                    data["ResponseText"] = new StreamReader(context.ResponseStream).ReadToEnd();
                    context.ResponseStream.Seek(0L, SeekOrigin.Begin);
                }
            }
            catch
            {
                // it does not matter much if it fails
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
