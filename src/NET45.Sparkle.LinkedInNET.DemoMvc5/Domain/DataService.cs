
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web;

    public class DataService
    {
        private IDataStore<RootData> store;
        private RootData data;

        public DataService(IDataStore<RootData> store)
        {
            this.store = store;
        }

        private RootData Data
        {
            get
            {
                if (this.data == null)
                {
                    this.data = this.store.Read() ?? new RootData();
                }

                return this.data;
            }
        }

        public bool HasAccessToken
        {
            get { return this.Data.AccessToken != null; }
        }

        public void SaveAccessToken(string accessToken)
        {
            using (var transaction = this.store.Write())
            {
                transaction.Data.AccessToken = accessToken;
            }
        }

        internal string GetAccessToken()
        {
            return this.Data.AccessToken;
        }

        internal void ClearAccessToken()
        {
            using (var transaction = this.store.Write())
            {
                transaction.Data.AccessToken = null;
            }
        }

        [DataContract]
        public class RootData
        {
            [DataMember]
            public string AccessToken { get; set; }
        }
    }
}