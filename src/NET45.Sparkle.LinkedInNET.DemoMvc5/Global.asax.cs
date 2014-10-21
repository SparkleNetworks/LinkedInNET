
namespace Sparkle.LinkedInNET.DemoMvc5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Ninject;
    using Ninject.Web.Common;
    using Sparkle.LinkedInNET.DemoMvc5.Domain;

    public class MvcApplication : NinjectHttpApplication //// : System.Web.HttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
        ////}
        ////protected void Application_Start()
        ////{
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // couldn't figure how to make ninject work easily
            // injection did not happen after installing package Ninject.Mvc3/5
            // used http://stackoverflow.com/a/14027722/282105 as a workaround
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory(this.Kernel));
        }

        protected override Ninject.IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Bind<DataService>().ToSelf().InRequestScope().WithConstructorArgument<IDataStore<DataService.RootData>>(new JsonFileDataStore<DataService.RootData>(this.Server.MapPath("~/App_Data/data.json")));
            return kernel;
        }
    }
}
