using System;
using System.Web.Mvc;
using System.Web.Routing;
using Rantup.Data.Infrastructure;

namespace Rantup.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnlyEnabledAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var accountId = filterContext.HttpContext.User.Identity.Name;

            if (SecurityCheckShouldBeExecuted(filterContext))
            {
                if (!UserCanAccessAdminArea(accountId))
                {
                    var redirectValues = new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" }, { "area", null } };

                    filterContext.Result = new RedirectToRouteResult(redirectValues);
                }
            }
        }

        private bool SecurityCheckShouldBeExecuted(AuthorizationContext filterContext)
        {
            // If current action or controller is marked as OnlyAdmin, security check should be executed
            return filterContext.ActionDescriptor.IsDefined(typeof(OnlyEnabledAttribute), true) ||
                   filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(OnlyEnabledAttribute), true);
        }

        private bool UserCanAccessAdminArea(string userId)
        {
            return DependencyManager.Repository.GetAccount(userId).Enabled;
        }
    }
}