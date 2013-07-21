using System.Web;
using System.Web.Routing;

namespace iMenyn.Web.Handlers
{
    public class CombinerRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new CombinerHandler();
        }
    }
}