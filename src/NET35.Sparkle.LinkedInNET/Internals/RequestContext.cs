
namespace Sparkle.LinkedInNET.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    internal class RequestContext
    {
        private Dictionary<string, string> queryStrings;
        private Dictionary<string, string> postQueryStrings;

        public string Method { get; set; }

        public string UrlPath { get; set; }

        public Dictionary<string, string> QueryStrings
        {
            get { return this.queryStrings; }
        }

        public Dictionary<string, string> PostQueryStrings
        {
            get { return this.postQueryStrings; }
        }

        internal void AddUrlArgumentToUrlQueryString(string key, string value)
        {
            if (this.queryStrings == null)
                this.queryStrings = new Dictionary<string, string>();

            this.queryStrings.Add(key, value);
        }

        internal void AddUrlArgumentToPostContent(string key, string value)
        {
            if (this.postQueryStrings == null)
                this.postQueryStrings = new Dictionary<string, string>();

            this.postQueryStrings.Add(key, value);
        }
    }
}
