using System.Web.Mvc;

namespace iMenyn.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AdminOLD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "AdminOLD/{action}/{id}",
                new { controller= "AdminOLD", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
