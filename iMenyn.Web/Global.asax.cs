using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Concrete;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;
using iMenyn.Data.Objects;
using iMenyn.Web.App_Start;
using iMenyn.Web.Infrastructure;

namespace iMenyn.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var ravenContext = RavenContext.Instance;

            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Allow ninject to take care of dependency injection in controllers
            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());

            //Custom bindings
            DependencyManager.InjectRavenDbContext(ravenContext);
            DependencyManager.NinjectKernel.Bind<IAuthentication>().To<Authentication>();
            FirstTimeSetup();

            InitializeRavenProfiler();
        }

        private void InitializeRavenProfiler()
        {
            Raven.Client.MvcIntegration.RavenProfiler.InitializeFor(RavenContext.Instance.DocumentStore);
        }

        private static void FirstTimeSetup()
        {
            // Wait until database is not stale anymore
            RavenContext.Instance.DocumentStore.ClearStaleIndexes();

            if (!DependencyManager.Db.Accounts.GetAccounts().Any())
            {
                var account = new Account
                {
                    Name = "Jesse",
                    Email = "jessetinell@gmail.com",
                    IsAdmin = true,
                    Enabled = true,
                    Enterprise = null
                };
                account.SetPassword("qwerty");
                DependencyManager.Db.Accounts.AddAccount(account);
            }
        }
    }
}