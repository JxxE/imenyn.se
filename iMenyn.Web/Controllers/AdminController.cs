﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using iMenyn.Data.Abstract;
using iMenyn.Data.Abstract.Db;
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
            : base(db,logger)
        {
        }

        public ActionResult Index()
        {
            var model = new DashboardViewModel
            {
                NewEnterprises = new List<Enterprise>(),
                ModifiedMenus = new List<string>()
            };

            if (CurrentAccount.IsAdmin)
            {
                var newEnterprises = Db.Enterprises.GetNewEnterprises();
                model.NewEnterprises = newEnterprises.ToList();

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
            return View(model);

            //TODO
            //var model = new DashboardViewModel
            //{
            //    NewEnterprises = new List<Enterprise>(),
            //    EnterprisesWithModifiedMenus = new List<Enterprise>(),
            //    //StandardViewModel = new StandardViewModel()
            //};

            //if (CurrentAccount.IsAdmin)
            //{
            //    var newEnterprises = Repository.GetNewEnterprises().Take(5);
            //    //var modifiedMenus = Repository.GetAllModifiedMenus();

            //    var enterprisesWithModifiedMenus = Repository.GetEnterprisesWithModifiedMenus();

            //    model.NewEnterprises = newEnterprises.ToList();
            //    model.EnterprisesWithModifiedMenus = enterprisesWithModifiedMenus.ToList();
            //}
            //else
            //{
            //    if(CurrentAccount.Enterprise == null)
            //    {
            //        return View(model);
            //    }

            //    var enterprise = Repository.GetEnterpriseById(CurrentAccount.Enterprise);

            //    if(enterprise.Menu != null)
            //    {
            //        var menu = Repository.GetMenuById(enterprise.Menu);
            //        var products = Repository.GetProducts(menu.Products.ToList());

            //        //var s = ViewModelHelper.CreateStandardViewModel(enterprise, products);
            //        //model.StandardViewModel = s;
            //    }
            //}
            //return View(model);
            //return View();
            //return null;
        }

        public ActionResult Manage()
        {
            var model = new DashboardViewModel
            {
                NewEnterprises = new List<Enterprise>(),
                ModifiedMenus = new List<string>()
            };

            if (CurrentAccount.IsAdmin)
            {
                var newEnterprises = Db.Enterprises.GetNewEnterprises();
                model.NewEnterprises = newEnterprises.ToList();

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
            return null;

        }

        public ActionResult Accounts()
        {
            //TODO
            //var accounts = Repository.GetAccounts().Where(a => a.IsAdmin != true);
            //ViewBag.Accounts = accounts;
            return null;
        }

        public ActionResult Settings()
        {
            return null;
        }

        #region Modified menus
        //Show all enterprises with modified menus
        public ActionResult ModifiedMenus()
        {
            //TODO
            return null;
            //var modifiedMenus = Repository.GetAllModifiedMenus();
            //var enterprises = modifiedMenus.Select(modifiedMenu => Repository.GetEnterpriseById(modifiedMenu.EnterpriseId)).ToList();

            //var model = new AllEnterprisesViewModel
            //{
            //    Enterprises = enterprises
            //};
            //return View(model);
        }
        public ActionResult ModifiedMenu(string enterpriseKey)
        {
            //var enterpriseId = EnterpriseHelper.GetId(enterpriseKey);

            //var modifiedMenu = Repository.GetModifiedMenuByEnterpriseId(enterpriseId);

            //var enterprise = Repository.GetEnterpriseById(enterpriseId);

            //if (enterprise != null)
            //{
            //    var liveMenu = Repository.GetMenuById(enterprise.Menu);
            //    if (liveMenu != null)
            //    {
            //        var modifiedProductsViewModel = new ProductListViewModel();
            //        var modifiedMenuId = "";

            //        if (modifiedMenu != null)
            //        {
            //            var modifiedMenuProducts = Repository.GetProducts(modifiedMenu.ProductIds).ToList();
            //            modifiedProductsViewModel = ViewModelHelper.GetProductListViewModel(modifiedMenuProducts);
            //            modifiedMenuId = modifiedMenu.Id;
            //        }

            //        var liveMenuProducts = Repository.GetProducts(liveMenu.Products.ToList()).ToList();
            //        var liveMenuProductsViewModel = ViewModelHelper.GetProductListViewModel(liveMenuProducts);


            //        var viewModel = new CompareModifiedMenuViewModel
            //        {
            //            Enterprise = enterprise,
            //            ModifiedMenuProducts = modifiedProductsViewModel,
            //            LiveMenuProducts = liveMenuProductsViewModel,
            //            ModifiedMenuId = modifiedMenuId
            //        };
            //        return View(viewModel);
            //    }
            //}
            return View();
        }
        #endregion

        [HttpPost]
        public ActionResult CreateAllIndexes()
        {
            RavenContext.Instance.CreateAllIndexes();
            ViewBag.Message = "Återskapade index";
            return null;
        }


        //Display the new temp-menu
        public ActionResult NewMenu(string enterpriseId)
        {
            //var enterprise = Repository.GetEnterpriseById(enterpriseId);
            //var model = new StandardViewModel();

            //if (enterprise.Menu != null)
            //{
            //    var menu = Repository.GetMenuById(enterprise.Menu);
            //    var products = Repository.GetProducts(menu.Products.ToList());
            //    //model = ViewModelHelper.CreateStandardViewModel(enterprise, products);
            //}
            //else
            //{
            //    var p = new List<Product>();
            //   // model = ViewModelHelper.CreateStandardViewModel(enterprise, p);
            //}

            return null;
        }


        public RedirectToRouteResult ApproveModifiedMenu(string enterpriseKey)
        {
            //TODO
            //var modifiedMenuId = ModifiedMenuHelper.GetId(enterpriseKey);
            //var modifiedMenu = Repository.GetModifiedMenuById(modifiedMenuId);

            //if (modifiedMenu == null) return RedirectToAction("Index");

            //var liveMenuId = MenuHelper.GetId(enterpriseKey);
            //var liveMenu = Repository.GetMenuById(liveMenuId);

            //if (liveMenu == null) return RedirectToAction("Index");

            //var liveMenuProductList = liveMenu.Products.ToList();
            //var oldProductIds = new List<string>();

            //foreach (var productId in liveMenuProductList)
            //{
            //    oldProductIds.Add(productId);
            //}

            //liveMenuProductList.Clear();
            //liveMenuProductList.AddRange(modifiedMenu.ProductIds);

            //liveMenu.Products = liveMenuProductList;

            ////Repository.UpdateMenu(liveMenu);
            ////Repository.DeleteModifiedMenuById(modifiedMenuId);
            ////Repository.DeleteProductsByIds(oldProductIds);
            return null;
            //return RedirectToAction("ModifiedMenu", "Admin", new { enterpriseKey });

        }
        public RedirectToRouteResult DisapproveModifiedMenu(string modifiedMenuId)
        {
            //TODO
            //Repository.DeleteModifiedMenuById(modifiedMenuId);
            return RedirectToAction("ModifiedMenus");
        }


        //Convert the temp to a real one.
        public RedirectToRouteResult ApproveMenu(string enterpriseId)
        {
            //TODO
            return null;
            //var enterprise = Repository.GetEnterpriseById(enterpriseId);

            //enterprise.IsTemp = false;

            //Repository.UpdateEnterprise(enterprise);

            //return RedirectToAction("NewEnterprises");
        }

        //Delete the temporary menu.
        public RedirectToRouteResult DisapproveMenu(string enterpriseId)
        {
            //TODO
            return null;
            //var enterprise = Repository.GetEnterpriseById(enterpriseId);
            //Repository.DeleteEnterpriseById(enterprise.Id);

            //if (enterprise.Menu != null)
            //{
            //    var menu = Repository.GetMenuById(enterprise.Menu);
            //    Repository.DeleteMenuById(menu.Id);
            //    Repository.DeleteProductsByIds(menu.Products.ToList());
            //}

            //return RedirectToAction("NewEnterprises");
        }

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
                                      Id = Data.Helpers.AccountHelper.GetId(account.UserInput.Email),
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
    }
}
