﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Infrastructure;

namespace iMenyn.Web.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OnlyAdminAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var accountId = filterContext.HttpContext.User.Identity.Name;

            if (!SecurityCheckShouldBeExecuted(filterContext)) return;
            if (UserCanAccessAdminArea(accountId)) return;
            var redirectValues = new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" }, { "area", null } };

            filterContext.Result = new RedirectToRouteResult(redirectValues);
        }

        private bool SecurityCheckShouldBeExecuted(AuthorizationContext filterContext)
        {
            // If current action or controller is marked as OnlyAdmin, security check should be executed
            return filterContext.ActionDescriptor.IsDefined(typeof(OnlyAdminAttribute), true) ||
                   filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(OnlyAdminAttribute), true);
        }

        private bool UserCanAccessAdminArea(string userId)
        {
            //TODO
            //return DependencyManager.Repository.GetAccount(userId).IsAdmin;
            return false;
        }
    }
}