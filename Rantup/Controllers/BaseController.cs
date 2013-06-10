using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Infrastructure;

namespace Rantup.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IRepository Repository;
        protected IAuthentication Authentication;

        //protected Account CurrentAccount
        //{
        //    get { return _currentAccount ?? (_currentAccount = AccountHelper.GetCurrentAccount()); }
        //}

        //private Account _currentAccount;

        public BaseController(IRepository repository, IAuthentication authentication = null)
        {
            Repository = repository;

            // Allows us to injects a IUserHelper in unit tests
            Authentication = authentication ?? DependencyManager.Authentication;
        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        //Account currentUser = Repository.GetUser(HttpContext.User.Identity.Name);

        //        //if (currentUser == null || !currentUser.Enabled)
        //        //{
        //        //    // If currentUser hasn't at least one valid account we sign out the user and then redirect to login page.
        //        //    // This can happend when a user is signed in but the user is deleted by the account admin.
        //        //    Authentication.SignOut();
        //        //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "LogOn" } });
        //        //}
        //        //else
        //        //{
        //        //    ViewBag.CurrentUser = currentUser;

        //        //    var currentAccount = GetCurrentAccount();

        //        //    ViewBag.CurrentAccount = currentAccount;
        //        //}
        //    }
        //    base.OnActionExecuting(filterContext);
        //}

    }
}
