using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Web.Models;

namespace Rantup.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IRepository repository) : base(repository)
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
            var user = Repository.GetUserByEmail(model.UserName);

            if (user == null || !user.ValidatePassword(model.Password))
            {
                ModelState.AddModelError("UserNotExistOrPasswordNotMatch", "Matchar inte");
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

        private ActionResult RedirectFromLoginPage(string retrunUrl = null)
        {
            if (string.IsNullOrEmpty(retrunUrl) || retrunUrl.Contains("AutoLogOn"))
                return RedirectToAction("Index", "Admin");
            return Redirect(retrunUrl);
        }

        public ActionResult LogOff()
        {
            Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
