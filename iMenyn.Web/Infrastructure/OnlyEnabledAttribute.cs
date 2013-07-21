using System;
using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Web.Infrastructure
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
                    var redirectValues = new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" }, { "area", null } };

                    filterContext.Result = new RedirectToRouteResult(redirectValues);
                }
            }
        }

        private static bool SecurityCheckShouldBeExecuted(AuthorizationContext filterContext)
        {
            // If current action or controller is marked as OnlyEnabled, security check should be executed
            return filterContext.ActionDescriptor.IsDefined(typeof(OnlyEnabledAttribute), true) ||
                   filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(OnlyEnabledAttribute), true);
        }

        private static bool UserCanAccessAdminArea(string userId)
        {
            return !string.IsNullOrEmpty(userId) && DependencyManager.Repository.GetAccount(userId).Enabled;
        }
    }
}