
namespace Sparkle.LinkedInNET.DemoMvc5.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Ninject.Modules;

    public class LinkedInApiModule : NinjectModule
    {
        public override void Load()
        {
            var config = LinkedInApiConfiguration.FromAppSettings("MyDemo.LinkedInConnect");
            this.Bind<LinkedInApi>().To<LinkedInApi>().InSingletonScope().WithConstructorArgument<LinkedInApiConfiguration>(config);
        }
    }
}