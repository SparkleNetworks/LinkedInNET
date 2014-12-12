
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
            this.Bind<LinkedInApiConfiguration>().ToMethod(_ => LinkedInApiConfiguration.FromAppSettings("MyDemo.LinkedInConnect"));
            this.Bind<LinkedInApi>().ToSelf().InSingletonScope();
        }
    }
}