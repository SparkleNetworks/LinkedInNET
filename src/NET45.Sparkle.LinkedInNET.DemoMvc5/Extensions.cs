
namespace Sparkle.LinkedInNET.DemoMvc5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public static class Extensions
    {
        /// <summary>
        /// Create the left part of a URL based on a HTTP request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string Compose(this HttpRequestBase httpRequest)
        {
            string url = httpRequest.IsSecureConnection ? "https://" : "http://";

            url += httpRequest.ServerVariables["SERVER_NAME"];

            if (httpRequest.ServerVariables["SERVER_PORT"] == "443" && httpRequest.IsSecureConnection || httpRequest.ServerVariables["SERVER_PORT"] == "80" && !httpRequest.IsSecureConnection)
            {
            }
            else
            {
                url += ":" + httpRequest.ServerVariables["SERVER_PORT"];
            }

            return url;
        }

        public static MvcHtmlString DisplayUrl(this HtmlHelper html, string url)
        {
            string value;
            if (url.StartsWith("https://"))
            {
                value = url;
            }
            else if (url.StartsWith("http://"))
            {
                value = url;
            }
            else if (url.StartsWith("www."))
            {
                value = "http://" + url;
            }
            else
            {
                return new MvcHtmlString(url);
            }

            var tag = new TagBuilder("a");
            tag.AddCssClass("external");
            tag.Attributes.Add("href", value);
            tag.Attributes.Add("title", value);
            tag.SetInnerText(url);

            return MvcHtmlString.Create(tag.ToString());
        }

        public static long ToUnixTime(this DateTime value)
        {
            var unix = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            if (value < unix)
                throw new ArgumentException("Specified value is lower than the UNIX time.");

            return (long)value.Subtract(unix).TotalMilliseconds;
        }

        public static DateTime FromUnixTime(this long value)
        {
            var unix = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return unix.AddMilliseconds(value);
        }
    }
}