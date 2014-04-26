using System.Web.Mvc;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Web.Models;

namespace iMenyn.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IDb db, ILogger logger, IAuthentication authentication = null)
            : base(db, logger, authentication)
        {
        }
        #region Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "/")
        {
            if (ControllerContext.HttpContext.Request.IsAuthenticated)
                return RedirectFromLoginPage();

            return View(new LogOnModel { ReturnUrl = returnUrl });
        }

        //
        // POST: /Administrate/LogOn

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LogOnModel model)
        {
            var user = Db.Accounts.GetAccountByEmail(model.UserName);

            if (user == null || !user.ValidatePassword(model.Password))
            {
                ModelState.AddModelError("UserNotExistOrPasswordNotMatch", "Fel lösenord");
            }
            else if (!user.Enabled)
            {
                ModelState.AddModelError("NotEnabled", "Kontot är inte aktiverat!");
            }

            if (ModelState.IsValid && user != null)
            {
                Authentication.SetAuthCookie(user.Id, true);
                return RedirectFromLoginPage(model.ReturnUrl);
            }

            return View(new LogOnModel { UserName = model.UserName, ReturnUrl = model.ReturnUrl });
        }

        public ActionResult Logout()
        {
            Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectFromLoginPage(string retrunUrl = null)
        {
            if (string.IsNullOrEmpty(retrunUrl) || retrunUrl.Contains("AutoLogOn"))
                return RedirectToAction("Index", "Home");
            return Redirect(retrunUrl);
        }
        #endregion
    }
}
