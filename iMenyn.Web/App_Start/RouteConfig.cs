using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Web.Handlers;

namespace iMenyn.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.Add(new Route("{folder}/Combiner", new CombinerRouteHandler()));
            RouteTable.Routes.Add(new Route("{folder}/{folder2}/{folder3}/Combiner", new CombinerRouteHandler()));

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //{controller}/{action}
            routes.MapRoute(
                "Default",
                "{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );


            //routes.MapRoute("Id", "{id}", new { controller = "Home", action = "Index" });
        }
    }
}