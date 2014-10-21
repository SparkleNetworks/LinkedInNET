
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class DataService
    {
        private IDataStore store;

        public DataService(IDataStore store)
        {
            this.store = store;
        }


    }
}