using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
using iMenyn.Data.Attributes;
using iMenyn.Data.Concrete;
using iMenyn.Data.Helpers;
using iMenyn.Data.Models;
using iMenyn.Web.Areas.Admin.ViewModels;
using iMenyn.Web.Helpers;
using iMenyn.Web.ViewModels;

namespace iMenyn.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        public AdminController(IDb db, ILogger logger)
            : base(db, logger)
        {
        }

        public ActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                NewEnterprises = new List<Enterprise>(),
                ModifiedEnterprises = new List<Enterprise>(),
                Account = AccountViewHelper.ModelToViewModel(CurrentAccount)
            };

            if (CurrentAccount.IsAdmin)
            {
                var modifiedAndNewEnterprises = Db.Enterprises.GetModifiedAndNewEnterprises().ToList();

                var newEnterprises = modifiedAndNewEnterprises.Where(e => e.IsNew);
                var modifiedEnterprises = modifiedAndNewEnterprises.Where(e => e.LockedFromEdit);

                viewModel.NewEnterprises = newEnterprises.ToList();
                viewModel.ModifiedEnterprises = modifiedEnterprises.ToList();
                viewModel.AllEnterprises = Db.Enterprises.GetAllEnterprises().OrderBy(e => e.Name);

                viewModel.ProductCount = Db.Products.ProductTotalCount();
            }
            else
            {
                //if (CurrentAccount.Enterprise == null)
                //{
                //    return View(model);
                //}

                //var enterprise = Repository.GetEnterpriseById(CurrentAccount.Enterprise);

                //if (enterprise.Menu != null)
                //{
                //    var menu = Repository.GetMenuById(enterprise.Menu);
                //    var products = Repository.GetProducts(menu.Products.ToList());

                //var s = ViewModelHelper.CreateStandardViewModel(enterprise, products);
                //model.StandardViewModel = s;
                //}
            }
            return View(viewModel);
        }

        [AdminOnly]
        public ActionResult Accounts()
        {
            //TODO
            //var accounts = Repository.GetAccounts().Where(a => a.IsAdmin != true);
            //ViewBag.Accounts = accounts;
            return null;
        }

        public ActionResult Log()
        {
            return View();
        }

        [AdminOnly]
        public ActionResult Settings()
        {
            return null;
        }

        [AdminOnly]
        public ActionResult CreateAllIndexes()
        {
            RavenContext.Instance.CreateAllIndexes();
            return RedirectToAction("Index");
        }


        [AdminOnly]
        public ActionResult MenuApproval(string enterpriseId)
        {
            var enterpriseViewModel = Db.Enterprises.GetCompleteEnterprise(enterpriseId, true);
            if (enterpriseViewModel.Enterprise.IsNew || enterpriseViewModel.Enterprise.LockedFromEdit)
                return View(enterpriseViewModel);

            return RedirectToAction("Index");
        }

        [AdminOnly]
        public RedirectToRouteResult MenuApprovalFunction(string enterpriseId, bool approved)
        {
            var enterprise = Db.Enterprises.GetEnterpriseById(enterpriseId);

            //Approve new menu
            if (approved && enterprise.IsNew)
            {
                enterprise.IsNew = false;
                Db.Enterprises.UpdateEnterprise(enterprise);
                Logger.Info("New enterprise approved: {0} ({1}), Code:[ccxcd77mnm]", enterprise.Name, enterpriseId);
            }

            //Disapprove new menu
            if (!approved && enterprise.IsNew)
            {
                Db.Enterprises.DeleteEnterprise(enterprise.Id);
            }

            //Approve modified menu
            if (approved && enterprise.LockedFromEdit && !enterprise.IsNew)
            {
                Db.Enterprises.SetModifiedMenuAsDefault(enterpriseId);
            }
            //Disapprove modified menu
            if (!approved && enterprise.LockedFromEdit && !enterprise.IsNew)
            {
               Db.Enterprises.DisapproveModifiedMenu(enterpriseId);
            }

            return RedirectToAction("Index");
        }

        [AdminOnly]
        [HttpPost]
        public ActionResult AddAccount(AccountViewData account)
        {
            if (account.UserInput.Password != account.UserInput.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Lösenorden matchar inte!");
            }
            if (ModelState.IsValid)
            {
                var newAccount = new Account
                                  {
                                      Id = AccountHelper.GetId(account.UserInput.Email),
                                      Email = account.UserInput.Email,
                                      Enabled = true,
                                      IsAdmin = false,
                                      Name = account.UserInput.Name,
                                  };
                newAccount.SetPassword(account.UserInput.Password);
                //Repository.AddAccount(newAccount);
            }

            //TODO
            //var accounts = Repository.GetAccounts().Where(a => a.IsAdmin != true);
            //ViewBag.Accounts = accounts;

            return null;
        }

        [AdminOnly]
        public RedirectToRouteResult DeleteEnterprise(string enterpriseId)
        {
            Db.Enterprises.DeleteEnterprise(enterpriseId);
            return RedirectToAction("Index");
        }

        #region Create fake enterprise
        [AdminOnly]
        public RedirectToRouteResult CreateFakeEnterprise(bool modified)
        {
            FakeDataHelper.CreateFakeEnterprise(Db, modified);
            return RedirectToAction("Index");
        }
        #endregion

        [AdminOnly]
        public JsonResult GetLogList(string query, bool info, bool debug, bool error, bool fatal, bool warn)
        {
            var loglist = Logger.GetLogList(query, info, debug, error, fatal, warn);
            return Json(loglist);
        }

    }
}
