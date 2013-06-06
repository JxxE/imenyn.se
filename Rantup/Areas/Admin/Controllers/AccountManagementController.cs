using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rantup.Data.Abstract;
using Rantup.Data.Concrete;
using Rantup.Data.Models;
using Rantup.Web.Areas.Admin.ViewModels;
using Rantup.Web.Controllers;
using Rantup.Web.Infrastructure;

namespace Rantup.Web.Areas.Admin.Controllers
{
    [OnlyAdmin]
    public class AccountManagementController : BaseController
    {
        //
        // GET: /Admin/Account/

        public AccountManagementController(IRepository repository, IAuthentication authentication = null) : base(repository, authentication)
        {
        }

        public ActionResult Index()
        {
            var accounts = Repository.GetAccounts().Where(a=>a.IsAdmin != true);
            ViewBag.Accounts = accounts;
            return View();
        }

        public ActionResult AddAccountView()
        {
            return View();
        }

        public ActionResult AccountDetails(string id)
        {
            var account = Repository.GetAccount(id);
            var model = new AccountViewModel
                            {
                                Id =account.Id,
                                Name = account.Name,
                                IsAdmin = account.IsAdmin,
                                Phone = account.Phone,
                                Email = account.Email,
                                Enabled = account.Enabled
                            };
            return View(model);
        }

        [HttpPost]
        public RedirectToRouteResult AddAccount(Account account)
        {
            var newAccount = new Account
                                 {
                                     Name = account.Name,
                                     Email = account.Email,
                                     Phone = account.Phone,
                                     Enabled = true,
                                     IsAdmin = false
                                 };
            newAccount.SetPassword("qwerty");
            Repository.AddAccount(newAccount);
            return RedirectToAction("AddAccountView");
        }

        public RedirectToRouteResult DisableAccount(string id)
        {
            EnableDisableAccount(id, false);
            return RedirectToAction("Index");
        }

        public ActionResult EnableAccount(string id)
        {
            EnableDisableAccount(id, true);
            return RedirectToAction("Index");
        }

        #region Metoder
        private void EnableDisableAccount(string id, bool enable)
        {
            var account = Repository.GetAccount(id);
            if (account == null) return;

            account.Enabled = enable;

            Repository.UpdateAccount(account);
        }
        #endregion
    }
}
