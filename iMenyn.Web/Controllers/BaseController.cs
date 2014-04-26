using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;
using iMenyn.Web.Helpers;

namespace iMenyn.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IDb Db;
        protected ILogger Logger;
        protected IAuthentication Authentication;

        protected Account CurrentAccount
        {
            get { return _currentAccount ?? (_currentAccount = AccountHelper.GetCurrentAccount()); }
        }
        private Account _currentAccount;

        public BaseController(IDb db, ILogger logger, IAuthentication authentication = null)
        {
            Db = db;
            Logger = logger;

            Authentication = authentication ?? DependencyManager.Authentication;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentAccount = AccountHelper.GetCurrentAccount();

                if (currentAccount == null || !currentAccount.Enabled)
                {
                    // If currentAccount hasn't isnt enabled we sign out the user and then redirect to login page.
                    Authentication.SignOut();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                }
                else
                {
                    ViewBag.IsAdmin = currentAccount.IsAdmin;
                }

            }
            ViewBag.IsAdmin = false;

            base.OnActionExecuting(filterContext);
        }

    }
}
