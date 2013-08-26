﻿using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Concrete;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;
using iMenyn.Web.App_Start;
using iMenyn.Web.Infrastructure;

namespace iMenyn.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var ravenContext = RavenContext.Instance;

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // Allow ninject to take care of dependency injection in controllers
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            // Custom bindings
            DependencyManager.InjectRavenDbContext(ravenContext);
            DependencyManager.NinjectKernel.Bind<IAuthentication>().To<Authentication>();
            FirstTimeSetup();
        }

        private void FirstTimeSetup()
        {
            if (DependencyManager.Repository.GetAccounts().Any()) return;
            
            var account = new Account
            {
                Name = "Jesse",
                Email = "jessevem@gmail.com",
                IsAdmin = true,
                Enabled = true,
                Enterprise = null
            };
            account.SetPassword("qwerty");
            DependencyManager.Repository.AddAccount(account);
        }

    }
}