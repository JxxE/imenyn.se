using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Infrastructure;
using iMenyn.Data.Models;
using iMenyn.Web.Helpers;
using iMenyn.Web.Infrastructure;

namespace iMenyn.Web.Areas.Admin.Controllers
{
    [OnlyEnabled]
    public class AdminBaseController : Controller
    {
        protected IAuthentication Authentication;

        protected Account CurrentAccount
        {
            get { return _currentAccount ?? (_currentAccount = AccountViewHelper.GetCurrentAccount()); }
        }

        private Account _currentAccount;

        public AdminBaseController( IAuthentication authentication = null)
        {
            

            // Allows us to injects a IUserHelper in unit tests
            Authentication = authentication ?? DependencyManager.Authentication;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var currentAccount = AccountViewHelper.GetCurrentAccount();

                if (currentAccount == null || !currentAccount.Enabled)
                {
                    // If currentAccount hasn't isnt enabled we sign out the user and then redirect to login page.
                    Authentication.SignOut();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "../Account" }, { "action", "Login" } });
                }
                else
                {
                    ViewBag.CurrentAccount = currentAccount;
                }
                   
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
