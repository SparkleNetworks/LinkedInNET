
namespace Sparkle.LinkedInNET.DemoMvc5.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Ninject.Modules;
    using System.Configuration;

    public class LinkedInApiModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<LinkedInApiConfiguration>().ToMethod(_ => FromAppSettings("MyDemo.LinkedInConnect"));
            this.Bind<LinkedInApi>().ToSelf().InSingletonScope();
        }

        private static LinkedInApiConfiguration FromAppSettings(string prefix)
        {
            if (prefix != null)
            {
                if (!prefix.EndsWith("."))
                    prefix += ".";
            }

            var me = new LinkedInApiConfiguration();
            me.ApiKey = ConfigurationManager.AppSettings[prefix + "ApiKey"];
            me.ApiSecretKey = ConfigurationManager.AppSettings[prefix + "ApiSecretKey"];

            return me;
        }
    }
}
