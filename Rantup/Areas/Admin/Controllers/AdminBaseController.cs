using System.Web.Mvc;
using System.Web.Routing;
using Rantup.Data.Abstract;
using Rantup.Data.Infrastructure;
using Rantup.Data.Models;
using Rantup.Web.Helpers;
using Rantup.Web.Infrastructure;

namespace Rantup.Web.Areas.Admin.Controllers
{
    [OnlyEnabled]
    public class AdminBaseController : Controller
    {
        protected IRepository Repository;
        protected IAuthentication Authentication;

        protected Account CurrentAccount
        {
            get { return _currentAccount ?? (_currentAccount = AccountHelper.GetCurrentAccount()); }
        }

        private Account _currentAccount;

        public AdminBaseController(IRepository repository, IAuthentication authentication = null)
        {
            Repository = repository;

            // Allows us to injects a IUserHelper in unit tests
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
