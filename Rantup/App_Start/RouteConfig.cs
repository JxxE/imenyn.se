using System.Web.Mvc;
using System.Web.Routing;
using Rantup.Web.Handlers;

namespace Rantup.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.Add(new Route("{folder}/Combiner", new CombinerRouteHandler()));
            RouteTable.Routes.Add(new Route("{folder}/{folder2}/{folder3}/Combiner", new CombinerRouteHandler()));

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