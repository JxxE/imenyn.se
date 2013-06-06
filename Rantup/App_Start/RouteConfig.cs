using System.Web.Mvc;
using System.Web.Routing;

namespace Rantup.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "UrlKey",
            //    url: "{urlKey}",
            //    defaults: new { controller = "Home", action = "Index", id = @"^[a-zA-Z0-9]+$" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


        }
    }
}